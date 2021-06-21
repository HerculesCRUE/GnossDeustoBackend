using Hercules.Asio.XML_RDF_Conversor.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.XML_RDF_Conversor.Models.Services
{
    public interface ICallUrisFactoryApiService
    {
        /// <summary>
        /// Obtiene una rui
        /// </summary>
        /// <param name="resourceClass">Resource class o rdfType</param>
        /// <param name="identifier">Identificador</param>
        /// <returns>uri</returns>
        public string GetUri(string resourceClass, string identifier);
    }
}
