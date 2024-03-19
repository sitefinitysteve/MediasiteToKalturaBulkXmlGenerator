#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("attachments")]
    public class Attachments
    {
        [XmlElement("attachment")]
        public List<Attachment> Items { get; set; } = new List<Attachment>();

    }

    [XmlType("attachment")]
    public class Attachment
    {
        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
        [XmlElement("filename")]
        public string Filename { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }
    }
}
