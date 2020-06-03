using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallUrisFactoryApiService
    {
        public string GetUri(string resourceClass, string identifier);
    }
}
