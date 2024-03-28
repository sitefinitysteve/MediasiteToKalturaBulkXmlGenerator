using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediasiteToKalturaXmlExport.Src
{
    public class Description
    {
        public string MediasiteId { get; set; }
        public string LegacyFolderStructure { get; set; }
        public List<string> Presenters { get; set; } = new List<string>();
        public List<Guid> FolderIds { get; set; } = new List<Guid>();
        public List<string> FolderNames { get; set; } = new List<string>();

        public DescriptionDTO GetDTO()
        {
            return new DescriptionDTO()
            {
                MediasiteId = this.MediasiteId,
                LegacyFolderStructure = this.LegacyFolderStructure,
                Presenters = this.Presenters
            };
        }
    }

    public class DescriptionDTO()
    {
        public string MediasiteId { get; set; }
        public string LegacyFolderStructure { get; set; }
        public List<string> Presenters { get; set; } = new List<string>();
    }
}
