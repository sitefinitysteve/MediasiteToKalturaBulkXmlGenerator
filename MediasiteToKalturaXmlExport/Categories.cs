#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Linq;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("categories")]
    public class Categories
    {
        [XmlElement("categoryId")]
        public List<string> Items { get; set; } = new List<string>();
    }
}
