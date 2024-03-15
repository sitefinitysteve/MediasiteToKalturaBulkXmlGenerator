#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ServiceStack.Diagnostics.Events;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("mrss")]
    public class Mrss
    {
        [XmlElement("channel")]
        public Channel Channel { get; set; } = new Channel();
    }

    [XmlType("channel")]
    public class Channel
    {
        [XmlElement("item")]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    [XmlType("item")]
    public class Item
    {
        [XmlIgnore]
        public bool Error { get; set; } = false;
        //DEFAULT PROPERTIES
        [XmlElement("action")]
        public string Action { get; set; } = "add";

        [XmlElement("type")]
        public int Type { get; set; } = 1;

        [XmlElement("userId")]
        public string UserId { get; set; } = "medvideo@mcmaster.ca";

        //PROPERTIES TO POPULATE FROM THE MEDIASITE XML
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("tags")]
        public Tags Tags { get; set; } = new Tags();

        [XmlElement("categories")]
        public Categories Categories { get; set; } = new Categories();

        [XmlElement("media")]
        public Media Media { get; set; } = new Media();

        [XmlElement("contentAssets")]
        public ContentAssets ContentAssets { get; set; } = new ContentAssets();

        [XmlElement("thumbnails")]
        public Thumbnails Thumbnails { get; set; } = new Thumbnails();

        [XmlIgnore]
        public List<Slide> Slides { get; set; } = new List<Slide>();
    }
}
