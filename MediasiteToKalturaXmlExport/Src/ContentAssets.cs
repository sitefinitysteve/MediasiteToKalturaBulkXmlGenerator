#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport.Src
{
    [XmlType("contentAssets")]
    public class ContentAssets
    {
        [XmlElement("content")]
        public Content Content { get; set; } = new Content();
    }

    [XmlType("content")]
    public class Content
    {
        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
    }

    [XmlType("urlContentResource")]
    public class Resource
    {
        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
