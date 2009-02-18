//-----------------------------------------------------------------------
// <copyright file="ContentTypes.cs" company="madd0 (http://www.madd0.com)">
//     Copyright (c) madd0 (http://www.madd0.com). All rights reserved.
// </copyright>
// <author>Mauricio DIAZ ORLICH (mauricio.diazorlich@gmail.com)</author>
//-----------------------------------------------------------------------

namespace Madd0.DocbookConverter
{
    /// <summary>
    /// Provides a set of predetermined MIME types to describe
    /// XAML package elements.
    /// </summary>
    public static class ContentTypes
    {
        /// <summary>
        /// Specifies a XAML MIME type.
        /// </summary>
        public static readonly string Xaml = "application/vnd.ms-wpf.xaml+xml";

        /// <summary>
        /// Specifies a Portable Network Graphics MIME type.
        /// </summary>
        public static readonly string Png = "image/png";

        /// <summary>
        /// Specifies a Bitmap MIME type.
        /// </summary>
        public static readonly string Bmp = "image/bmp";

        /// <summary>
        /// Specifies a GIF MIME type.
        /// </summary>
        public static readonly string Gif = "image/gif";

        /// <summary>
        /// Specifies a JPEG MIME type.
        /// </summary>
        public static readonly string Jpeg = "image/jpeg";

        /// <summary>
        /// Determines whether the specified MIME type is a XAML type.
        /// </summary>
        /// <param name="type">The MIME type to test.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is a XAML MIME type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsXamlType(string type)
        {
            return type.Equals(ContentTypes.Xaml);
        }

        /// <summary>
        /// Determines whether the specified MIME type is an image type.
        /// </summary>
        /// <param name="type">The MIME type to test.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is an image MIME type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageType(string type)
        {
            return type.Equals(ContentTypes.Bmp) ||
                   type.Equals(ContentTypes.Png) ||
                   type.Equals(ContentTypes.Gif) ||
                   type.Equals(ContentTypes.Jpeg);
        }
    }
}
