using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediasiteToKalturaXmlExport
{
    public class Description
    {
        public string MediasiteId { get; set; }
        public string LegacyFolderStructure { get; set; }
        public List<string> Presenters { get; set; } = new List<string>();
    }
}
