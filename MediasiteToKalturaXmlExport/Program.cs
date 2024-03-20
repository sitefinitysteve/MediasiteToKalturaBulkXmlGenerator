#pragma warning disable CS8602 // Dereference of a possibly null reference.

using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.Xml.Serialization;
using MediasiteToKalturaXmlExport;
using MediasiteToKalturaXmlExport.Src;
using Microsoft.Extensions.Configuration;
using System;


#region Config
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string _defaultKaltruaCategoryId = config.GetValue<string>("DefaultKalturaCategory");
string _basePath = config.GetValue<string>("RemoteBasePath");

if (!_basePath.EndsWith("/"))
{
    _basePath += "/";
}

var map = new ConfigurationBuilder()
    .AddJsonFile("mediasiteToKalturaMap.json", optional: false, reloadOnChange: true)
    .Build();

#endregion

var media = new Mrss();
var errors = new List<Item>();

var folders = System.IO.Directory.GetDirectories(config.GetValue<string>("RootMediasiteFolderToParse"));

//Make sure the export folder exists
if (!System.IO.Directory.Exists("export"))
{
    System.IO.Directory.CreateDirectory("export");
}

//Delete export files if they exist
if (System.IO.File.Exists(@"export\export.xml"))
{
    System.IO.File.Delete(@"export\export.xml");
}

if (System.IO.File.Exists(@"export\export_errors.xml"))
{
    System.IO.File.Delete(@"export\export_errors.xml");
}


var mediaSiteMaps = map.GetSection("Mapping").Get<List<MediaSiteMap>>();

//Loop through each folder
foreach (var f in folders)
{
    var folderName = System.IO.Path.GetFileName(f);
    //Remove .zip from the end of the folder if it exist
    if (folderName.EndsWith(".zip"))
    {
        folderName = folderName.Substring(0, folderName.Length - 4);
    }

    var encodedName = EncodeFolderName(folderName);
    var newItem = new Item();

    //Open the MediasitePresentation_70.xml file for reading
    var xml = System.IO.File.ReadAllText(f + @"\MediasitePresentation_70.xml");
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    var xdoc = XDocument.Parse(xml);

    //Get presentation title from the xdoc
    Guid msId = new Guid(
        xdoc.Element("LocalPresentationManifest")
            .Element("Properties")
            .Element("Presentation")
            .Element("Id")
            .Element("Value")
            .Value);

    string title = xdoc.Element("LocalPresentationManifest")
        .Element("Properties")
        .Element("Presentation")
        .Element("Title")
        .Value;
    
    newItem.Name = $"{config.GetValue<string>("VideoTitlePrefix")}{title}";
    
    newItem.Media.Items.Add("1");

    //Video URL
    newItem.ContentAssets.Content.Resource.Url = GetVideoAndSlides(_basePath, folderName, newItem, doc);
    if(!newItem.ContentAssets.Content.Resource.Url.Contains(".mp4"))
    {
        newItem.Error = true;
    }

    //Get description json
    var desc = GetDescription(doc, msId);
    
    newItem.Description = ServiceStack.Text.JsonSerializer.SerializeToString(desc.GetDTO());

    desc.FolderIds.Reverse();
    foreach (Guid guid in desc.FolderIds)
    {
        // Process each Guid in reverse order
        var mapItem = mediaSiteMaps.FirstOrDefault(x => x.Mediasite.StartsWith(guid.ToString().Replace("-","")));
        if(mapItem != null)
        {
            newItem.Categories.Items.Add(mapItem.Kaltura);
            break;
        }
    }

    //Add tags
    newItem.Tags.Items.Add("Medportal");
    newItem.Tags.Items.Add("Mediasite");

    foreach(var p in desc.Presenters)
    {
        newItem.Tags.Items.Add(p);
    }

    //Thumbnails
    var thumbNode = doc.SelectSingleNode("/LocalPresentationManifest/Properties/ContentRevisions/ContentRevision/ThumbnailContent/FileName");
    if(thumbNode != null)
    {
        var thumb = new Thumbnail();
        thumb.Resource.Url = $@"{_basePath}{encodedName}/Content/{thumbNode.InnerText}";
        thumb.IsDefault = true;

        newItem.Thumbnails.Items.Add(thumb);
    }

    //Look for captions
    var captionNode = doc.SelectSingleNode("//CaptionContent");
    if(captionNode != null)
    {
        var caption = new SubTitle();
        var captionFileName = captionNode.SelectSingleNode("FileName").InnerText;
        caption.Resource.Url = $@"{_basePath}{encodedName}/Content/{captionFileName}";

        if (captionFileName.ToLower().EndsWith("srt"))
        {
            caption.Format = 1;
        }else if (captionFileName.ToLower().EndsWith("dfxp"))
        {
            caption.Format = 2;
        }
        else if (captionFileName.ToLower().EndsWith("webvtt"))
        {
            caption.Format = 3;
        }
        else if (captionFileName.ToLower().EndsWith("cap"))
        {
            caption.Format = 4;
        }
        else if (captionFileName.ToLower().EndsWith("scc"))
        {
            caption.Format = 5;
        }

        caption.Tags.Add(new SubTitleTag() { Tag = $"Format {caption.Format}" });

        newItem.SubTitle.Items.Add(caption);
    }

    //Add the item to the media object
    if (newItem.Error)
    {
        errors.Add(newItem);
    } else
    {
        media.Channel.Items.Add(newItem);
    }
}

//Write success xml to filesystem
string xmlResult;
#region Success
using (StringWriter stringwriter = new System.IO.StringWriter())
{
    var serializer = new XmlSerializer(media.GetType());
    serializer.Serialize(stringwriter, media);
    xmlResult = stringwriter.ToString();
}

var prettyDoc = XDocument.Parse(xmlResult);
System.IO.File.WriteAllText(@"export\export.xml", prettyDoc.ToString());
#endregion

#region Errors
using (StringWriter stringwriter = new System.IO.StringWriter())
{
    var serializer = new XmlSerializer(errors.GetType());
    serializer.Serialize(stringwriter, errors);
    xmlResult = stringwriter.ToString();
}

prettyDoc = XDocument.Parse(xmlResult);
System.IO.File.WriteAllText(@"export\export_errors.xml", prettyDoc.ToString());
#endregion


Console.WriteLine($"Success: {media.Channel.Items.Count}");
Console.WriteLine($"Errors: {errors.Count}");



static string GetVideoAndSlides(string _basePath, string folderName, Item newItem, XmlDocument doc)
{
    XDocument xdoc = XDocument.Parse(doc.OuterXml);
    string fileName = "";
    var encodedName = EncodeFolderName(folderName);

    //Select all nodes named "ContentStream"

    var streams = xdoc.Descendants("Streams");

    var contentNodes = streams.Descendants("ContentStream");

    XElement? videoNodes;
    if (contentNodes.Count() == 1)
    {
        //videoNodes = contentNodes.Cast<XmlNode>().Where(x => x != null);
        videoNodes = contentNodes.Descendants("StreamType").Select(x => x.Parent).FirstOrDefault();
    }
    else
    {
        //Filter nodes to where the StreamType has value 4
        videoNodes = contentNodes.Descendants("StreamType").Where(x => x.Value == "4").Select(x => x.Parent).FirstOrDefault();
    }

    var mp4Nodes = new List<XElement>();
    foreach(var p in xdoc.Descendants("PresentationContent"))
    {
        if(p.Element("MimeType")?.Value == "video/mp4")
        {
            mp4Nodes.Add(p);
        }
    }

    //Order the mp4Nodes by the FileLength element
    mp4Nodes = mp4Nodes.OrderBy(x => x.Element("Length")?.Value).ToList();

    var smallestNode = mp4Nodes.FirstOrDefault();
   
    newItem.MsDuration = Convert.ToInt32(smallestNode.Element("Length")?.Value);

    //Get the filename from the videoNode
    fileName = smallestNode?.Element("FileName")?.Value ?? "";

    //Slides
    var slideContent = videoNodes.Descendants("SlideContent").FirstOrDefault();
    if (slideContent != null)
    {
        var slides = slideContent.Descendants("Slide");
        if(slides.Count() > 0)
        {
            var filenameBase = slideContent.Element("FileName")?.Value;
            foreach(var slide in slides)
            {
                var index = int.Parse(slide.Element("Number").Value);

                var newSlide = new SceneCuePoint()
                {
                     Title = $"Slide {index}",
                };

                
                var slideTime = int.Parse(slide.Element("Time").Value);

                newSlide.SceneStartTime = MillisecondsToTimeString(slideTime);
                newSlide.Description = $"Slide {index} @ {newSlide.SceneStartTime}";
                

                //Filename base has {0:D4} in it, I need to get the number after the D to know the digits
                var digits = int.Parse(filenameBase.Substring(filenameBase.IndexOf("{0:D") + 4, 1));

                //The indexname now needs to be as long as the digit number, like 0001 or 0010
                
                var slideFilename = $"{filenameBase.Substring(0, filenameBase.IndexOf("{0:D"))}{index.ToString($"D{digits}")}_full.jpg";
                newSlide.Scene.Resource.Url = $@"{_basePath}{encodedName}/Content/{slideFilename}";
                
                newItem.Scenes.CuePoints.Add(newSlide);
            }
        }
    }

    var videoUrl = $@"{_basePath}{encodedName}/Content/{fileName}";
    return videoUrl;
   
}

static Description GetDescription(XmlDocument doc, Guid msId)
{
    Description description = new Description();
    //Loop through all the folders to build the old path
    var oldPath = "";
    var folderNodes = doc.SelectNodes("/LocalPresentationManifest/Properties/Folders/Folder");
    foreach (XmlNode folderNode in folderNodes)
    {
        var folderTitle = folderNode.SelectSingleNode("Name").InnerText;
        if (folderTitle.ToLower() != "mediasite" && folderTitle.ToLower() != "medportal")
        {
            oldPath += $"/{folderTitle}";
            description.FolderIds.Add(new Guid(folderNode.SelectSingleNode("Id//Value").InnerText));
        }
    }
    description.LegacyFolderStructure = oldPath;
    description.MediasiteId = msId.ToString();

    //Get presenters
    var presenters = doc.SelectNodes("/LocalPresentationManifest/Properties/Presentation/Presenters/PresentationPresenter");
    foreach (XmlNode presenter in presenters)
    {
        var prefix = presenter.SelectSingleNode("Prefix")?.InnerText;
        var firstName = presenter.SelectSingleNode("FirstName").InnerText;
        var lastName = presenter.SelectSingleNode("LastName").InnerText;

        //Combine values, ignore prefix if empty
        var presenterName = string.IsNullOrEmpty(prefix) ? $"{firstName} {lastName}" : $"{prefix} {firstName} {lastName}";
        description.Presenters.Add(presenterName);
    }


    return description;
}

static string MillisecondsToTimeString(int milliseconds)
{
    int seconds = milliseconds / 1000;
    int minutes = seconds / 60;
    int hours = minutes / 60;
    seconds %= 60;
    minutes %= 60;

    return $"{hours:00}:{minutes:00}:{seconds:00}";
}

static string EncodeFolderName(string folderName)
{
    return Uri.EscapeDataString(folderName);
}