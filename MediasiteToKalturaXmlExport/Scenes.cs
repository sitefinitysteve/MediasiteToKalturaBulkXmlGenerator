#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Serialization;

namespace MediasiteToKalturaXmlExport
{
    [XmlType("scenes")]
    public class Scenes
    {
        [XmlElement("scene-thumb-cue-point")]
        public List<SceneCuePoint> CuePoints { get; set; } = new List<SceneCuePoint>();

    }

    [XmlType("scene-thumb-cue-point")]
    public class SceneCuePoint
    {
        [XmlElement("sceneStartTime")]
        public string SceneStartTime { get; set; }

        [XmlElement("tags", IsNullable = false)]
        public List<SubTitleTag> Tags { get; set; } = new List<SubTitleTag>();

        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("slide")]
        public Slide Scene { get; set; } = new Slide();
    }

    [XmlType("slide")]
    public class Slide
    {

        [XmlElement("urlContentResource")]
        public Resource Resource { get; set; } = new Resource();
    }
}
