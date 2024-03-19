#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("slides")]
    public class Slides
    {
        [XmlElement("slide")]
        public List<Slide> Items { get; set; } = new List<Slide>();

    }

    [XmlType("slide")]
    public class Slide
    {
        [XmlElement("index")]
        public int Index { get; set; }
        [XmlElement("filename")]
        public string Filename { get; set; }
        [XmlElement("time")]
        public int Time { get; set; }
        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
    }
}
