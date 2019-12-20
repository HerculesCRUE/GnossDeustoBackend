using System;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public static class ConfigJsonHandler
    {
        private static UriStructureGeneral _uriStructure;
        public static void InitializerConfigJson()
        {
            if (_uriStructure == null)
            {
                try
                {
                    _uriStructure = ReaderConfigJson.Read();
                }
                catch (Exception)
                {
                    throw new FailedLoadConfigJsonException("Could not load config file, maybe Config/UrisConfig.json does not exist or is bad formed");
                }
            }
        }

        public static UriStructureGeneral GetUriStructure()
        {
            if(_uriStructure == null)
            {
                InitializerConfigJson();
            }
            return _uriStructure;
        }
    }
}
