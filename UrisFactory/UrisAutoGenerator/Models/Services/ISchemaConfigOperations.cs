using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrisFactory.Models.Services
{
    public interface ISchemaConfigOperations
    {
        public abstract string GetContentType();
        public abstract byte[] GetFileSchemaData();
        public abstract bool SaveConfigFile(IFormFile formFile);
        public abstract bool SaveConfigJson();
    }
}
