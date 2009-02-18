<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl xaml"
    xmlns:xaml="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:param name="hasMedia" />
  <xsl:param name="pathPrefix" />
  <xsl:param name="hasRoot" />
  
  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="xaml:Section">
    <section>
      <xsl:apply-templates />
    </section>
  </xsl:template>

  <xsl:template match="xaml:Paragraph">
    <para>
      <xsl:apply-templates />
    </para>
  </xsl:template>

  <xsl:template match="xaml:Run[@FontWeight='Bold']">
    <emphasis role="bold">
      <xsl:apply-templates />
    </emphasis>
  </xsl:template>
  
  <xsl:template match="xaml:Run[@FontStyle='Italic']">
    <emphasis>
      <xsl:apply-templates />
    </emphasis>
  </xsl:template>
  
  <xsl:template match="xaml:List[@MarkerStyle='Disc']">
    <itemizedlist>
      <xsl:apply-templates />
    </itemizedlist>
  </xsl:template>

  <xsl:template match="xaml:List[@MarkerStyle='Decimal']">
    <orderedlist>
      <xsl:apply-templates />
    </orderedlist>
  </xsl:template>

  <xsl:template match="xaml:ListItem">
    <listitem>
      <xsl:apply-templates />
    </listitem>
  </xsl:template>

  <xsl:template match="xaml:BlockUIContainer">
    <xsl:if test="$hasMedia">
      <mediaobject>
        <xsl:apply-templates />
      </mediaobject>
    </xsl:if>
  </xsl:template>
  
  <xsl:template match="xaml:InlineUIContainer">
    <xsl:if test="$hasMedia">
      <inlinemediaobject>
        <xsl:apply-templates />
      </inlinemediaobject>
    </xsl:if>
  </xsl:template>

  <xsl:template match="xaml:Image">
    <imageobject>
      <xsl:element name="imagedata">
        <xsl:attribute name="fileref"><xsl:value-of select="$pathPrefix" /><xsl:value-of select="substring-after(.//xaml:BitmapImage/@UriSource,'/')"/></xsl:attribute>
        <xsl:attribute name="width"><xsl:value-of select="round(//xaml:Image/@Width)"/></xsl:attribute>
      </xsl:element>
    </imageobject>
  </xsl:template>
</xsl:stylesheet>
