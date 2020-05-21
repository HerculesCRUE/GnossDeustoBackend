using System;
using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class ConfigJsonHandler
    {
        private UriStructureGeneral _uriSchema;

        public ConfigJsonHandler()
        {
            InitializerConfigJson();
        }

        public ConfigJsonHandler(string json)
        {
            InitializerConfigJson(json);
        }

        private void InitializerConfigJson()
        {
            if (_uriSchema == null) 
            {
                LoadConfigJson();
            }
        }

        private void InitializerConfigJson(string json)
        {
            if (_uriSchema == null)
            {
                LoadConfigJson(json);
            }
        }

        public UriStructureGeneral GetUrisConfig()
        {
            if(_uriSchema == null)
            {
                InitializerConfigJson();
            }
            return _uriSchema;
        }

        public void LoadConfigJson()
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

        public void LoadConfigJson(string json)
        {
            try
            {
                _uriSchema = ReaderConfigJson.Read(json);
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

        private bool IsCorrectFormedUriStructure()
        {
            bool correct = false;
            if (_uriSchema != null && _uriSchema.Base != null && _uriSchema.Characters.Count > 0 && _uriSchema.UriStructures.Count > 0)
            {
                correct = true;
            }
            else
            {
                _uriSchema = null;
            }
            return correct;
        }

        public static bool IsCorrectFormedUriStructure(UriStructureGeneral uriSchema)
        {
            bool correct = false;
            if (uriSchema != null && uriSchema.Base != null && uriSchema.Characters.Count > 0 && uriSchema.UriStructures.Count > 0)
            {
                correct = true;
            }
            else
            {
                uriSchema = null;
            }
            return correct;
        }

        //Operations with the Schema
        private void DeleteUriStructureInfo(UriStructure uriStructure, List<ResourcesClass> resourcesClass)
        {
            _uriSchema.UriStructures.Remove(uriStructure);
            foreach(ResourcesClass item in resourcesClass)
            {
                _uriSchema.ResourcesClasses.Remove(item);
            }
           
        }

        ///<exception cref="UriStructureConfiguredException">UriStructure not exist in config file</exception>
        public void DeleteUriStructureInfo(string name)
        {
            if (ExistUriStructure(name))
            {
                var uriStructure = _uriSchema.UriStructures.First(uriStructure => uriStructure.Name.Equals(name));
                var resourcesClasses = _uriSchema.ResourcesClasses.Where(uriStructure => uriStructure.ResourceURI.Equals(name)).ToList();
                DeleteUriStructureInfo(uriStructure, resourcesClasses);
            }
            else
            {
                throw new UriStructureConfiguredException($"No data of uriStructure {name}");
            }
            
        }

        public bool ExistUriStructure(string name)
        {
            return _uriSchema.UriStructures.Any(uriStructure => uriStructure.Name.Equals(name));
        }

        public UriStructure GetUriStructure(string name)
        {
            return _uriSchema.UriStructures.FirstOrDefault(uriStruct => uriStruct.Name.Equals(name));
        }
        
        public List<ResourcesClass> GetResourceClass(string name)
        {
            return _uriSchema.ResourcesClasses.Where(resourceClass => resourceClass.ResourceURI.Equals(name)).ToList();
        }

        ///<exception cref="UriStructureConfiguredException">UriStructure Already exist in config file</exception>
        ///<exception cref="UriStructureBadInfoException">there is a mismatch between uriStructure and resourceClass given</exception>
        public void AddUriStructureInfo(UriStructure uriStructure, ResourcesClass resourcesClass)
        {

            if (!_uriSchema.UriStructures.Any(uriStructures => uriStructures.Name.Equals(uriStructure))  &&(!string.IsNullOrEmpty(uriStructure.Name) && uriStructure.Name.Equals(resourcesClass.ResourceURI)) && (uriStructure.Components.Count>1 && !string.IsNullOrEmpty(resourcesClass.LabelResourceClass) && !string.IsNullOrEmpty(resourcesClass.ResourceClass)))
            {
                _uriSchema.UriStructures.Add(uriStructure);
                _uriSchema.ResourcesClasses.Add(resourcesClass);
            }
            else if(ExistUriStructure(uriStructure.Name))
            {
                throw new UriStructureConfiguredException($"UriStructure {uriStructure.Name} already exist");
            }
            else if(!uriStructure.Name.Equals(resourcesClass.ResourceURI))
            {
                throw new UriStructureBadInfoException($"UriStructure name: {uriStructure.Name} and ResourcesClass ResourceURI: {resourcesClass.ResourceURI} no match");
            }
            else
            {
                throw new UriStructureBadInfoException($"Data component is empty");
            }
        }

       
    }
}
