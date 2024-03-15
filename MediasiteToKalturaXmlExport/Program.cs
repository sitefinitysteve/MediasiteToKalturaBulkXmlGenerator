#pragma warning disable CS8602 // Dereference of a possibly null reference.

//Loop through all folders in C:\GitHub\Medportal\Medportal.Sitefinity\Files\Mediasite

//Get all folders under /Files/MediaSite
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using MediasiteToKalturaXmlExport;

const string _rootCategoryId = "331852402";
const string _basePath = "https://www.medportal.ca/Files/Mediasite/";

var media = new Mrss();
var errors = new List<Item>();

var folders = System.IO.Directory.GetDirectories(@"C:\GitHub\Medportal\Medportal.Sitefinity\Files\Mediasite");

//Delete files if they exist
if (System.IO.File.Exists(@"..\..\..\export\export.xml"))
{
    System.IO.File.Delete(@"..\..\..\export\export.xml");
}

if (System.IO.File.Exists(@"..\..\..\export\export_errors.xml"))
{
    System.IO.File.Delete(@"..\..\..\export\export_errors.xml");
}

//Loop through each folder
foreach (var f in folders)
{
    var folderName = System.IO.Path.GetFileName(f);
    var encodedName = EncodeFolderName(folderName);
    var newItem = new Item();

    //Open the MediasitePresentation_70.xml file for reading
    var xml = System.IO.File.ReadAllText(f + @"\MediasitePresentation_70.xml");
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    var xdoc = XDocument.Parse(xml);

    //Get presentation title from the xdoc
    Guid msId = new Guid(xdoc.Element("LocalPresentationManifest")
                          .Element("Properties")
                          .Element("Presentation")
                          .Element("Id")
                          .Element("Value").Value);

    string title = xdoc.Element("LocalPresentationManifest")
                          .Element("Properties")
                          .Element("Presentation")
                          .Element("Title").Value;

    newItem.Name = title;


    newItem.Categories.Items.Add(_rootCategoryId);

    newItem.Media.Items.Add("1");

    //Video URL
    newItem.ContentAssets.Content.Resource.Url = GetVideoAndSlides(_basePath, folderName, newItem, doc);
    if(!newItem.ContentAssets.Content.Resource.Url.Contains(".mp4"))
    {
        newItem.Error = true;
    }

    //Get description json
    var desc = GetDescription(doc, msId); ;
    newItem.Description = ServiceStack.Text.JsonSerializer.SerializeToString(desc);

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
        thumb.Resource.Url = $@"{_basePath}{encodedName}\Content\{thumbNode.InnerText}";
        newItem.Thumbnails.Items.Add(thumb);

        //Set the default thumbnail
        newItem.Thumbnails.IsDefault = true;
    }

    //Add the item to the media object
    if (newItem.Error)
    {
        errors.Add(newItem);
    }
    else
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
System.IO.File.WriteAllText(@"..\..\..\export\export.xml", prettyDoc.ToString());
#endregion

#region Errors
using (StringWriter stringwriter = new System.IO.StringWriter())
{
    var serializer = new XmlSerializer(errors.GetType());
    serializer.Serialize(stringwriter, errors);
    xmlResult = stringwriter.ToString();
}

prettyDoc = XDocument.Parse(xmlResult);
System.IO.File.WriteAllText(@"..\..\..\export\export_errors.xml", prettyDoc.ToString());
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
    if (streams.Count() == 1)
    {
        //videoNodes = contentNodes.Cast<XmlNode>().Where(x => x != null);
        videoNodes = contentNodes.Descendants("StreamType").Select(x => x.Parent).FirstOrDefault();
    }
    else
    {
        //Filter nodes to where the StreamType has value 4
        videoNodes = contentNodes.Descendants("StreamType").Where(x => x.Value == "4").Select(x => x.Parent).FirstOrDefault();
    }

    //Get the child node "PresentationContent" nodes regardless of depth where the MediaType is video/mp4
    var presentationContent = videoNodes?.Descendants("PresentationContent").Where(x => x.Element("MimeType")?.Value == "video/mp4").FirstOrDefault();
    
    //Get the filename from the videoNode
    fileName = presentationContent?.Element("FileName")?.Value ?? "";

    //Slides
    var slideContent = videoNodes.Descendants("SlideContent").FirstOrDefault();
    if (slideContent != null)
    {
        var slides = slideContent.Descendants("Slide");
        if(slides.Count() > 0)
        {
            var filenameBase = slideContent.Element("FileName")?.Value;
            
            foreach (var slide in slides)
            {
                var newSlide = new Slide();
                newSlide.Index = int.Parse(slide.Element("Number").Value);
                newSlide.Time = int.Parse(slide.Element("Time").Value);

                //Filename base has {0:D4} in it, I need to get the number after the D to know the digits
                var digits = int.Parse(filenameBase.Substring(filenameBase.IndexOf("{0:D") + 4, 1));

                //The indexname now needs to be as long as the digit number, like 0001 or 0010
                newSlide.Filename = $"{filenameBase.Substring(0, filenameBase.IndexOf("{0:D"))}{newSlide.Index.ToString($"D{digits}")}.jpg";

                newItem.Slides.Add(newSlide);
            }
        }
    }

    var videoUrl = $@"{_basePath}{encodedName}\Content\{fileName}";
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
        }
    }
    description.LegacyFolderStructure = oldPath;
    description.MediasiteId = msId.ToString();

    //Get presenters
    var presenters = doc.SelectNodes("/LocalPresentationManifest/Properties/Presentation/Presenters/PresentationPresenter");
    foreach (XmlNode presenter in presenters)
    {
        var prefix = presenter.SelectSingleNode("Prefix").InnerText;
        var firstName = presenter.SelectSingleNode("FirstName").InnerText;
        var lastName = presenter.SelectSingleNode("LastName").InnerText;

        //Combine values, ignore prefix if empty
        var presenterName = string.IsNullOrEmpty(prefix) ? $"{firstName} {lastName}" : $"{prefix} {firstName} {lastName}";
        description.Presenters.Add(presenterName);
    }


    return description;
}

static string EncodeFolderName(string folderName)
{
    return Uri.EscapeDataString(folderName);
}