// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Esta clase sirve para gestionar el fichero de configuración de las uris
using System;
using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    ///<summary>
    ///Esta clase sirve para gestionar el fichero de configuración de las uris
    ///</summary>
    public class ConfigJsonHandler
    {
        private UriStructureGeneral _uriSchema;

        ///<summary>
        ///Este constructor inicializa las clases con la información necesaria a partir del fichero json
        ///</summary>
        public ConfigJsonHandler()
        {
            InitializerConfigJson();
        }

        ///<summary>
        ///Este constructor inicializa las clases con la información necesaria a partir de una cadena de texto emulando el json del fichero de configuracion
        ///</summary>
        ///<param name="json">cadena de texto emulando el json del fichero de configuracion</param>
        public ConfigJsonHandler(string json)
        {
            InitializerConfigJson(json);
        }

        ///<summary>
        ///Método para cargar la clase general de estructuras Uri a partir del fichero json de configuración
        ///</summary>
        private void InitializerConfigJson()
        {
            if (_uriSchema == null) 
            {
                LoadConfigJson();
            }
        }

        ///<summary>
        ///Método para cargar la clase general de estructuras Uri a partir de una cadena de texto emulando el json del fichero de configuracion
        ///</summary>
        ///<param name="json">cadena de texto emulando el json del fichero de configuracion</param>
        private void InitializerConfigJson(string json)
        {
            if (_uriSchema == null)
            {
                LoadConfigJson(json);
            }
        }

        ///<summary>
        ///Devuelve el objeto con la estructura general del fichero de configuracion
        ///</summary>
        public UriStructureGeneral GetUrisConfig()
        {
            if(_uriSchema == null)
            {
                InitializerConfigJson();
            }
            return _uriSchema;
        }

        ///<summary>
        ///carga el objeto con la estructura general a partir del fichero de configuracion
        ///</summary>
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

        ///<summary>
        ///carga el objeto con la estructura general a partir de una cadena de texto emulando el json del fichero de configuracion
        ///</summary>
        ///<param name="json">cadena de texto emulando el json del fichero de configuracion</param>
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

        ///<summary>
        ///Comprueba que el objeto cargado tenga los elementos adecuados
        ///</summary>
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

        ///<summary>
        ///Comprueba que el la estructura general de uris pasado por parametros sea correcto cargado tenga los elementos adecuados
        ///</summary>
        ///<param name="uriSchema">estructura general de uris a comprobar su validez</param>
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
        ///<summary>
        ///Elimina del objeto de uris, una estructura uri con sus resourceClass asociadas
        ///</summary>
        ///<param name="uriStructure">estructura de uris a añadir</param>
        ///<param name="resourcesClass">lista de resource class asociadas a uriStructure</param>
        private void DeleteUriStructureInfo(UriStructure uriStructure, List<ResourcesClass> resourcesClass)
        {
            _uriSchema.UriStructures.Remove(uriStructure);
            foreach(ResourcesClass item in resourcesClass)
            {
                _uriSchema.ResourcesClasses.Remove(item);
            }
           
        }
        ///<summary>
        ///Elimina del objeto de uris, una estructura uri a partir de su nombre
        ///</summary>
        ///<param name="name">nombre de la estructura uri a eliminar</param>
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

        ///<summary>
        ///Comprueba que existe una estrucutra de uri
        ///</summary>
        ///<param name="name">nombre de la estructura uri a comprobar si existe</param>
        public bool ExistUriStructure(string name)
        {
            return _uriSchema.UriStructures.Any(uriStructure => uriStructure.Name.Equals(name));
        }

        ///<summary>
        ///Obtiene una estructura uri
        ///</summary>
        ///<param name="name">nombre de la estructura uri a traer</param>
        public UriStructure GetUriStructure(string name)
        {
            return _uriSchema.UriStructures.FirstOrDefault(uriStruct => uriStruct.Name.Equals(name));
        }

        ///<summary>
        ///Obtiene una lista de ResourceClass asociadas a una estructura uri
        ///</summary>
        ///<param name="name">nombre de la estructura uri</param>
        public List<ResourcesClass> GetResourceClass(string name)
        {
            return _uriSchema.ResourcesClasses.Where(resourceClass => resourceClass.ResourceURI.Equals(name)).ToList();
        }

        ///<summary>
        ///Añade del objeto de uris una estructura de uris y una resource class asiciada a esa estrcutura
        ///</summary>
        ///<param name="uriStructure">estructura uri</param>
        ///<param name="resourcesClass">resource class asociada a uriStructure</param>
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
