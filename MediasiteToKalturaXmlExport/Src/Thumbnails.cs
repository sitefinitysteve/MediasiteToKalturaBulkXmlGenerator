﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport.Src
{
    [XmlType("thumbnails")]
    public class Thumbnails
    {
        [XmlElement("thumbnail")]
        public List<Thumbnail> Items { get; set; } = new List<Thumbnail>();

    }

    [XmlType("thumbnail")]
    public class Thumbnail
    {
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; } = false;

        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
    }
}
