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
        public List<string> ResumptionTokenCustomParameterNames { get; set; }
        public List<MetadataFormat> MetadataFormats { get; set; }
        public List<Set> Sets { get; set; }
    }
}
