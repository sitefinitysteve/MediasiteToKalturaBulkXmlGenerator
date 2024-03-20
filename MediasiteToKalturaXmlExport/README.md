# Mediasite to Kaltura Exporter

## Generates the XML to send to the [Kaltura Media Console](https://kmc.kaltura.com/)


mediasiteToKalturaMap.json format example

This is the mediasite category folder id and kaltura category id.  The idea is you can arbitrarily place videos around in kaltura instead of having to adhear to the same old mediasite structure.

```
{
  "Mapping": [
    {
      "Mediasite": "c3b1876ede3248b29728b0cd352b217c14",
      "Kaltura": "338278332"
    },
  ]
}
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

| Property                   | Description                                                                                                                                                     |
|----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| RootMediasiteFolderToParse | Where is the local folder that contains all the exported video child folders                                                                                    |
| RemoteBasePath             | The base route to tell Kaltura to look for the files, so <path>/VideoFolder/...                                                                                 |
| DefaultKalturaCategory     | If there's no old mediasite category id link found, use this as the default category to dump the media into                                                     |
| VideoTitlePrefix           | (Optional) This is so the videos you test bulk upload can be found and deleted easier from the KMC.  When ready to run officially just blank out this property. |

## Important Links
* [Bulk Upload](https://knowledge.kaltura.com/help/uploading-and-ingestion)
* [Bulk Schema](https://www.kaltura.com/api_v3/xsdDoc/?type=bulkUploadXml.bulkUploadXML)
