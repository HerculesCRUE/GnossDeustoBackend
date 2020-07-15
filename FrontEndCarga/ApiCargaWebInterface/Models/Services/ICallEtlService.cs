using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallEtlService
    {
        public string CallGetRecord(Guid repoIdentifier, string identifier, string type);
        public void CallDataValidate(IFormFile rdf, Guid repositoryIdentifier);
        public void CallDataPublish(IFormFile rdf);
        public void PostOntology(IFormFile ontologyUri, int ontologyType);
        public string GetOntology(int ontologyType);
    }
}
