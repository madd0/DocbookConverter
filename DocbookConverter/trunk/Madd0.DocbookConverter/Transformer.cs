//-----------------------------------------------------------------------
// <copyright file="Transformer.cs" company="madd0 (http://www.madd0.com)">
//     Copyright (c) madd0 (http://www.madd0.com). All rights reserved.
// </copyright>
// <author>Mauricio DIAZ ORLICH (mauricio.diazorlich@gmail.com)</author>
//-----------------------------------------------------------------------

namespace Madd0.DocbookConverter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    /// <summary>
    /// The Transformer class is capable of manipulating XAML, XAML Packages and 
    /// Docbook documents and converting between these formats.
    /// </summary>
    public static class Transformer
    {
        // XslCompiledTransfoms stored as static field in order to reuse compiled XSLT
        private static XslCompiledTransform xamlToDocbookTransform;
        private static XslCompiledTransform docbookToXamlTransform;

        public static void ConvertXamlPackageToDocbook(Stream xamlPackage, string outputPath)
        {
            ConvertXamlPackageToDocbook(xamlPackage, outputPath, ".");
        }

        public static void ConvertXamlPackageToDocbook(Stream xamlPackage, string outputPath, string relativeAttachmentOutputPath)
        {
            using (Package p = Package.Open(xamlPackage))
            {
                var parts = p.GetParts();

                if (parts.Where(part => ContentTypes.IsXamlType(part.ContentType)).Count() != 1)
                {
                    throw new ArgumentException("Package can only contain one XAML document.", "xamlPackage");
                }

                var xamlPart = p.GetParts().Where(part => ContentTypes.IsXamlType(part.ContentType)).Single();

                using (Stream xaml = xamlPart.GetStream(FileMode.Open, FileAccess.Read))
                {
                    var docbook = Transformer.TransformXamlToDocbook(xaml, true, "./" + relativeAttachmentOutputPath + "/" + Path.GetFileName(outputPath) + "_");

                    using (Stream docbookFile = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        CopyStream(docbook, docbookFile);
                    }
                }

                var outputFolder = Path.GetDirectoryName(outputPath);

                var attachments = p.GetParts().Where(part => ContentTypes.IsImageType(part.ContentType));

                // TODO This code is ugly and probably buggy
                foreach (var attachment in attachments)
                {
                    var xamlUri = MakeAbsolutePath(new Uri(outputFolder), xamlPart.Uri);
                    var attPath = MakeAbsolutePath(new Uri(outputFolder), attachment.Uri);
                    var tt = xamlUri.MakeRelativeUri(attPath);

                    var _42 = Path.Combine(Path.Combine(outputFolder, relativeAttachmentOutputPath),
                                string.Format(CultureInfo.InvariantCulture, "{0}_{1}", Path.GetFileName(outputPath), tt));

                    if (!Directory.Exists(Path.GetDirectoryName(_42)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(_42));
                    }

                    using (Stream attStream = File.Open(_42, FileMode.Create, FileAccess.ReadWrite))
                    {
                        CopyStream(attachment.GetStream(FileMode.Open, FileAccess.Read), attStream);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a XAML package to docbook.
        /// </summary>
        /// <param name="xamlPackage">The xaml package.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="relativeAttachmentOutputPath">The attachment output path.</param>
        /// <returns></returns>
        public static Stream ConvertXamlPackageToDocbook(Stream xamlPackage)
        {
            using (Package p = Package.Open(xamlPackage))
            {
                var parts = p.GetParts();

                if (parts.Where(part => ContentTypes.IsXamlType(part.ContentType)).Count() != 1)
                {
                    throw new ArgumentException("Package can only contain one XAML document.", "xamlPackage");
                }

                var xamlPart = p.GetParts().Where(part => ContentTypes.IsXamlType(part.ContentType)).Single();

                using (Stream xaml = xamlPart.GetStream(FileMode.Open, FileAccess.Read))
                {
                    return Transformer.TransformXamlToDocbook(xaml, false);
                }
            }
        }

        public static Stream ConvertXamlToDocbook(Stream xaml)
        {
            return Transformer.TransformXamlToDocbook(xaml, false);
        }

        public static Stream ConvertDockbookToXaml(Stream docbook)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(docbook);

            var trans = Transformer.GetDocbookToXamlTransform();
            
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, null);
            writer.Formatting = Formatting.Indented;
            trans.Transform(doc, writer);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public static Stream ConvertDockbookToXamlPackage(Stream docbook, string attachmentsPath)
        {
            MemoryStream ms = new MemoryStream();

            Uri documentUri = PackUriHelper.CreatePartUri(new Uri("Xaml/Document.xaml", UriKind.Relative));

            using (Package p = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite))
            {
                PackagePart part = p.CreatePart(documentUri, ContentTypes.Xaml);

                CopyStream(ConvertDockbookToXaml(docbook), part.GetStream(FileMode.Open, FileAccess.Write));

                p.CreateRelationship(documentUri, TargetMode.Internal, "http://schemas.microsoft.com/wpf/2005/10/xaml/entry");

                // TODO Get external resources and add them to package
                docbook.Seek(0, SeekOrigin.Begin);
                var nav = new XPathDocument(docbook).CreateNavigator();

                var query = nav.Select("//@fileref");

                while (query.MoveNext())
                {
                    var attPath = Path.Combine(attachmentsPath, query.Current.Value);

                    if (File.Exists(attPath))
                    {
                        Uri partUri = PackUriHelper.CreatePartUri(new Uri("Xaml/" + query.Current.Value, UriKind.Relative));

                        var attPart = p.CreatePart(partUri, MimeType(attPath));

                        CopyStream(File.Open(attPath, FileMode.Open, FileAccess.Read), attPart.GetStream(FileMode.Open, FileAccess.Write));

                        p.CreateRelationship(partUri, TargetMode.Internal, "http://schemas.microsoft.com/wpf/2005/10/xaml/entry");
                        part.CreateRelationship(partUri, TargetMode.Internal, "http://schemas.microsoft.com/wpf/2005/10/xaml/component");

                    }
                }
            }

            ms.Seek(0, SeekOrigin.Begin);

#if DEBUG
            using (var debugOutput = File.Open(@"c:\Users\Madd0\Temp\debug.zip", FileMode.OpenOrCreate, FileAccess.Write))
            {
                CopyStream(ms, debugOutput);

                ms.Seek(0, SeekOrigin.Begin);
            }
#endif

            return ms;
        }

        private static string MimeType(string filename)
        {
            string mime = "application/octetstream";
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;
        } 
        
        private static Stream TransformXamlToDocbook(Stream xaml, bool hasMedia)
        {
            return Transformer.TransformXamlToDocbook(xaml, hasMedia, ".");
        }

        private static Stream TransformXamlToDocbook(Stream xaml, bool hasMedia, string relativeAttachmentPath)
        {
            var doc = new XmlDocument();
            doc.Load(xaml);

            var trans = Transformer.GetXamlToDocbookTransform();

            var args = new XsltArgumentList();
            args.AddParam("hasMedia", string.Empty, hasMedia);
            args.AddParam("pathPrefix", string.Empty, relativeAttachmentPath);
            
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, null);
            writer.Formatting = Formatting.Indented;
            trans.Transform(doc, args, writer);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        private static Uri MakeAbsolutePath(Uri baseUri, Uri relativeUri)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }
            else
            {
                if (!baseUri.AbsoluteUri.EndsWith("/", false, CultureInfo.InvariantCulture))
                {
                    baseUri = new Uri(baseUri.AbsoluteUri + "/");
                }

                if (relativeUri.OriginalString.StartsWith("/", true, CultureInfo.InvariantCulture))
                {
                    return new Uri(baseUri, "." + relativeUri.OriginalString);
                }
                else
                {
                    return new Uri(baseUri, relativeUri);
                }
            }
        }

        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }
        }

        #region XslCompiledTransform Creators
        private static XslCompiledTransform GetXamlToDocbookTransform()
        {
            if (Transformer.xamlToDocbookTransform == null)
            {
                Assembly a = Assembly.GetExecutingAssembly();

                using (Stream xslt = a.GetManifestResourceStream("Madd0.DocbookEditor.Xaml2Docbook.xslt"))
                {
                    Transformer.xamlToDocbookTransform = new XslCompiledTransform();
                    Transformer.xamlToDocbookTransform.Load(new XmlTextReader(xslt));
                }
            }

            return Transformer.xamlToDocbookTransform;
        }

        private static XslCompiledTransform GetDocbookToXamlTransform()
        {
            if (Transformer.docbookToXamlTransform == null)
            {
                Assembly a = Assembly.GetExecutingAssembly();

                using (Stream xslt = a.GetManifestResourceStream("Madd0.DocbookEditor.Docbook2Xaml.xslt"))
                {
                    Transformer.docbookToXamlTransform = new XslCompiledTransform();
                    Transformer.docbookToXamlTransform.Load(new XmlTextReader(xslt));
                }
            }

            return Transformer.docbookToXamlTransform;
        }
        #endregion
    }
}
