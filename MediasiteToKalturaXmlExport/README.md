# Mediasite to Kaltura Exporter

## Generates the XML to send to the [Kaltura Media Console](https://kmc.kaltura.com/)


mediasiteToKalturaMap.json format example

```
{
  "Mapping": [
    {
      "Mediasite": "c3b1876ede3248b29728b0cd352b217c14",
      "Kaltura": "338278332"
    },
```

appsettings.json properties
```
{
	"RootMediasiteFolderToParse": "C:\\Mediasite\\Files",
	"RemoteBasePath": "https://www.yoursite.ca/Files/Mediasite/", 
	"DefaultKalturaCategory": "331852402",
	"VideoTitlePrefix":  "BULKTEST - "
}
```



## Important Links
* [Bulk Upload](https://knowledge.kaltura.com/help/uploading-and-ingestion)
* [Bulk Schema](https://www.kaltura.com/api_v3/xsdDoc/?type=bulkUploadXml.bulkUploadXML)
