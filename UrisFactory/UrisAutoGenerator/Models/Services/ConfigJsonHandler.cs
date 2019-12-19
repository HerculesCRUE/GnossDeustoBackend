using System;
using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public static class ConfigJsonHandler
    {
        private static UriStructure _uriStructure;
        public static void InitializerConfigJson()
        {
            if (_uriStructure == null)
            {
                try
                {
                    _uriStructure = ReaderConfigJSON.Read();
                }
                catch(Exception ex)
                {
                    throw new FailedLoadConfigJsonException("Could not load config file, maybe Config/UrisConfig.json does not exist or is bad formed");
                }
            }
        }

        public static UriStructure GetUriStructure()
        {
            if(_uriStructure == null)
            {
                InitializerConfigJson();
            }
            return _uriStructure;
        }
    }
}
