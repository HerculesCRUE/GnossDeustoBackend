using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class DiscoverRdfViewModel
    {
        public List<string> urisRdf { get; set; }
        public string uriEntity { get; set; }

        public Dictionary<string, List<String>> stringPropertiesEntity { get; set; }
        public Dictionary<string, List<DiscoverRdfViewModel>> entitiesPropertiesEntity { get; set; }
        public Dictionary<string, string> communNamePropierties { get; set; }
        public List<string> LoadedEntities { get; set; }
    }
}
