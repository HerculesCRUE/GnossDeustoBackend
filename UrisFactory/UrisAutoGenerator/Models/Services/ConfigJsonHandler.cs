using System;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public static class ConfigJsonHandler
    {
        private static UriStructureGeneral _uriSchema;
        public static void InitializerConfigJson()
        {
            if (_uriSchema == null)
            {
                LoadConfigJson();
            }
        }

        public static UriStructureGeneral GetUrisConfig()
        {
            if(_uriSchema == null)
            {
                InitializerConfigJson();
            }
            return _uriSchema;
        }

        internal static void LoadConfigJson()
        {
            try
            {
                _uriSchema = ReaderConfigJson.Read();
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
            if (_uriSchema != null && _uriSchema.Base != null && _uriSchema.Characters.Count > 0 && _uriSchema.UriStructures.Count > 0)
            {
                correct = true;
            }
            return correct;
        }

        //Operations with the Schema
        public static void DeleteUriStructureInfo(UriStructure uriStructure, ResourcesClass resourcesClass)
        {
            _uriSchema.UriStructures.Remove(uriStructure);
            _uriSchema.ResourcesClasses.Remove(resourcesClass);
        }
        public static void DeleteUriStructureInfo(string name)
        {
            var uriStructure = _uriSchema.UriStructures.First(uriStructure => uriStructure.Name.Equals(name));
            var resourcesClasses = _uriSchema.ResourcesClasses.First(uriStructure => uriStructure.ResourceURI.Equals(name));
            DeleteUriStructureInfo(uriStructure, resourcesClasses);
        }

        public static bool ExistUriStructure(string name)
        {
            return _uriSchema.UriStructures.Any(uriStructure => uriStructure.Name.Equals(name));
        }

        public static UriStructure GetUriStructure(string name)
        {
            return _uriSchema.UriStructures.FirstOrDefault(uriStruct => uriStruct.Name.Equals(name));
        }
        
        public static ResourcesClass GetResourceClass(string name)
        {
            return _uriSchema.ResourcesClasses.FirstOrDefault(resourceClass => resourceClass.ResourceURI.Equals(name));
        }

        ///<exception cref="UriStructureConfiguredException">UriStructure Already exist in config file</exception>
        ///<exception cref="UriStructureBadInfoException">there is a mismatch between uriStructure and resourceClass given</exception>
        public static void AddUriStructureInfo(UriStructure uriStructure, ResourcesClass resourcesClass)
        {

            if (!_uriSchema.UriStructures.Any(uriStructures => uriStructures.Name.Equals(uriStructure))  && uriStructure.Name.Equals(resourcesClass.ResourceURI))
            {
                _uriSchema.UriStructures.Add(uriStructure);
                _uriSchema.ResourcesClasses.Add(resourcesClass);
            }
            else if(_uriSchema.UriStructures.Any(uriStructures => uriStructures.Name.Equals(uriStructure)))
            {
                throw new UriStructureConfiguredException();
            }
            else
            {
                throw new UriStructureBadInfoException();
            }
        }

       
    }
}
