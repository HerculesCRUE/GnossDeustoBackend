using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallUrisFactoryApiService
    {
        public string GetUri(string resourceClass, string identifier);
        public string GetSchema();
        public void ReplaceSchema(IFormFile newFile);
        public string GetStructure(string uriStructure);
        public void DeleteStructure(string uriStructure);
        public void AddStructure(string structure);
    }
}
