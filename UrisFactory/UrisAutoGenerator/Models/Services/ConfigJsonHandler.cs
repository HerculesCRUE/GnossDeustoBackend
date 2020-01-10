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
                LoadConfigJson();
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

        internal static void LoadConfigJson()
        {
            try
            {
                _uriStructure = ReaderConfigJson.Read();
                if (!IsCorrectFormedUriStructure())
                {
                    throw new FailedLoadConfigJsonException("Could not load config file, the structure is not correctly");
                }
            }
            catch (Exception)
            {
                throw new FailedLoadConfigJsonException("Could not load config file, maybe Config/UrisConfig.json does not exist or is bad formed");
            }
        }

        private static bool IsCorrectFormedUriStructure()
        {
            bool correct = false;
            if (_uriStructure != null && _uriStructure.Base != null && _uriStructure.Characters.Count > 0 && _uriStructure.UriStructures.Count > 0)
            {
                correct = true;
            }
            return correct;
        }
    }
}
