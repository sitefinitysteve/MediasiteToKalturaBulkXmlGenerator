#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("subTitles")]
    public class SubTitles
    {
        [XmlElement("subTitle")]
        public List<SubTitle> Items { get; set; } = new List<SubTitle>();

    }

    [XmlType("subTitle")]
    public class SubTitle
    { 
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; } = true;

        [XmlAttribute("label")]
        public string Label { get; set; } = "Mediasite";

        [XmlAttribute("lang")]
        public string Language { get; set; } = "English";

        [XmlAttribute("format")]
        public int Format { get; set; } = 2;

        [XmlElement("tags", IsNullable = false)]
        public List<SubTitleTag> Tags { get; set; } = new List<SubTitleTag>();

        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
    }

    [XmlType("tag")]
    public class SubTitleTag
    {
        [XmlElement("tag")]
        public string Tag { get; set; }
    }
}
