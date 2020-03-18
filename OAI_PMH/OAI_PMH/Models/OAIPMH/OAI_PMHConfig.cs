using System;
using System.Collections.Generic;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    public class OAI_PMHConfig
    {
        public bool SupportSets { get; set; }
        public string RepositoryName { get; set; }
        public string DeletedRecord { get; set; }
        public string[] AdminEmails { get; set; }
        public string Granularity { get; set; }
        public string EarliestDatestamp { get; set; }
        public string XML_CVN_Repository { get; set; }
        public List<MetadataFormat> MetadataFormats { get; set; }
        public List<Set> Sets { get; set; }
    }
}
