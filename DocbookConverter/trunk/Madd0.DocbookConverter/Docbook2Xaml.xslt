<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl sdf"
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:sdf="http://www.supinfo.com/sdf"
>
  <xsl:output method="xml" indent="yes"/>
  
  <xsl:template match="/">
    <Section xml:space="preserve" TextAlignment="Left" LineHeight="Auto" 
             IsHyphenationEnabled="False" FlowDirection="LeftToRight" NumberSubstitution.CultureSource="User" 
             NumberSubstitution.Substitution="AsCulture" FontFamily="Segoe UI" 
             FontStyle="Normal" FontWeight="Normal" FontStretch="Normal" 
             FontSize="12" Foreground="#FF000000" Typography.StandardLigatures="True" 
             Typography.ContextualLigatures="True" Typography.DiscretionaryLigatures="False" 
             Typography.HistoricalLigatures="False" Typography.AnnotationAlternates="0" 
             Typography.ContextualAlternates="True" Typography.HistoricalForms="False" 
             Typography.Kerning="True" Typography.CapitalSpacing="False" 
             Typography.CaseSensitiveForms="False" Typography.StylisticSet1="False" 
             Typography.StylisticSet2="False" Typography.StylisticSet3="False" 
             Typography.StylisticSet4="False" Typography.StylisticSet5="False" 
             Typography.StylisticSet6="False" Typography.StylisticSet7="False" 
             Typography.StylisticSet8="False" Typography.StylisticSet9="False" 
             Typography.StylisticSet10="False" Typography.StylisticSet11="False" 
             Typography.StylisticSet12="False" Typography.StylisticSet13="False" 
             Typography.StylisticSet14="False" Typography.StylisticSet15="False" 
             Typography.StylisticSet16="False" Typography.StylisticSet17="False" 
             Typography.StylisticSet18="False" Typography.StylisticSet19="False" 
             Typography.StylisticSet20="False" Typography.Fraction="Normal" 
             Typography.SlashedZero="False" Typography.MathematicalGreek="False" 
             Typography.EastAsianExpertForms="False" Typography.Variants="Normal" 
             Typography.Capitals="Normal" Typography.NumeralStyle="Normal" 
             Typography.NumeralAlignment="Normal" Typography.EastAsianWidths="Normal" 
             Typography.EastAsianLanguage="Normal" Typography.StandardSwashes="0" 
             Typography.ContextualSwashes="0" Typography.StylisticAlternates="0">
      <xsl:apply-templates />
    </Section>
  </xsl:template>


  <xsl:template match="para">
    <Paragraph>
      <xsl:apply-templates />
    </Paragraph>
  </xsl:template>

  <xsl:template match="para/programlisting">
      <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="programlisting">
    <Paragraph>
      <xsl:apply-templates />
    </Paragraph>
  </xsl:template>

  <xsl:template match="emphasis[@role='bold']">
    <Run FontWeight="Bold">
      <xsl:apply-templates />
    </Run>
  </xsl:template>
  
  <xsl:template match="emphasis">
    <Run FontStyle="Italic">
      <xsl:apply-templates />
    </Run>
  </xsl:template>
  
  <xsl:template match="itemizedList">
    <List MarkerStyle="Disc">
      <xsl:apply-templates />
    </List>
  </xsl:template>

  <xsl:template match="orderedList">
    <List MarkerStyle="Decimal">
      <xsl:apply-templates />
    </List>
  </xsl:template>

  <xsl:template match="listItem">
    <ListItem>
      <xsl:apply-templates />
    </ListItem>
  </xsl:template>

  <xsl:template match="mediaobject">
    <BlockUIContainer>
      <xsl:apply-templates />
    </BlockUIContainer>
  </xsl:template>

  <xsl:template match="inlinemediaobject">
    <InlineUIContainer>
      <xsl:apply-templates />
    </InlineUIContainer>
  </xsl:template>

  <xsl:template match="imageobject">
    <Image StretchDirection="DownOnly">
      <xsl:if test=".//imagedata/@width">
        <xsl:attribute name="Width"><xsl:value-of select=".//imagedata/@width"/></xsl:attribute>
      </xsl:if>
      <Image.Source>
        <xsl:element name="BitmapImage">
          <xsl:attribute name="UriSource"><xsl:value-of select=".//imagedata/@fileref"/></xsl:attribute>
          <xsl:attribute name="CacheOption">OnLoad</xsl:attribute>
        </xsl:element>
      </Image.Source>
    </Image>
  </xsl:template>
</xsl:stylesheet>