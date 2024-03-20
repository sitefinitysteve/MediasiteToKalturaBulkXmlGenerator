# Mediasite to Kaltura Exporter

## Generates the XML to send to the [Kaltura Media Console](https://kmc.kaltura.com/)

### Setup

1. Clone the repository	
2. Create the appsettings.json file in the root of the project (see below, set the properties)
3. Create the mediasiteToKalturaMap.json file in the root of the project (see below, set the properties)
4. Open in Visual Studio and run the project


### Configuration Files to generate

mediasiteToKalturaMap.json

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

appsettings.json

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

## Notes

* This was generated from our older version of mediasites XML, mileage may vary
* Attachments is commented out, we didn't have any videos with attachments to test with. But before the cuepoints worked we experimented with slides being attachments but abandoned that approach when there was a limit of 30 attachments per video.