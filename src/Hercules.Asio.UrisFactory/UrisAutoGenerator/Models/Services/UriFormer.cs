// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase encargada de generar las uris

using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    ///<summary>
    ///Clase encargada de generar las uris
    ///</summary>
    public class UriFormer
    {
        private UriStructureGeneral _uristructure;


        public UriFormer(UriStructureGeneral uristructure)
        {
            _uristructure = uristructure;
        }
        private UriStructureGeneral UriStructure
        {
            get
            {
                return _uristructure;    
            }
            
        }

        ///<summary>
        ///Devuelve una uri a partir de una resourceClass y y una lista de valores
        ///</summary>
        ///<param name="resourceClass">nombre de la resourceClass a usar para generar la uri</param>
        ///<param name="queryString">diccionario con los valores cogidos de la url de la petición</param>
        public string GetURI(string resourceClass, Dictionary<string, string> queryString, bool isRdfType = false)
        {
            string uri = "";
            ResourcesClass resourceClassObject = null;
            if (!isRdfType)
            {
                resourceClassObject = ParserResourceClass(resourceClass);
            }
            else
            {
                resourceClassObject = ParserResourceClassRdfType(resourceClass);
            }
            

            if (resourceClassObject != null)
            {
                string parsedLabelResourceClass = resourceClassObject.LabelResourceClass;
                string resourceURL = resourceClassObject.ResourceURI;

                UriStructure urlStructure = UriStructure.UriStructures.FirstOrDefault(structure => structure.Name.Equals(resourceURL));
                if (urlStructure != null)
                {
                    string parsedCharacter = ParserCharacter(urlStructure.Components.ToList());
                    if (!string.IsNullOrEmpty(parsedCharacter))
                    {
                        uri = GetUriByStructure(urlStructure, parsedCharacter, parsedLabelResourceClass, queryString);
                    }
                    else
                    {
                        throw new ParametersNotConfiguredException($"Character for {resourceURL} not configured");
                    }
                }
                else
                {
                    throw new ParametersNotConfiguredException($"Structure for {resourceURL} not configured");
                }
                return uri;
            }
            else
            {
                throw new ParametersNotConfiguredException($"resource class: '{resourceClass}' not configured");
            }
        }

        ///<summary>
        ///Genera una uri
        ///</summary>
        ///<param name="urlStructure">estructura URL para la construcción de la uri</param>
        ///<param name="parsedCharacter">Character a usar pra la generación de la uri, este character debe estar configurado en el fichero de configuración</param>
        ///<param name="parsedResourceClass">etiqueta a mostrar en la uri de la resource class de la cual queremos generar la uri</param>
        ///<param name="queryString">diccionario con los valores cogidos de la url de la petición</param>
        private string GetUriByStructure(UriStructure urlStructure, string parsedCharacter, string parsedResourceClass, Dictionary<string, string> queryString)
        {
            string uri = "";
            bool error = false;
            string errorMessage = "";
            bool containsKey = false;
            foreach (Component component in urlStructure.Components.OrderBy(structure => structure.UriComponentOrder))
            {
                string componentName = component.UriComponent;
                switch (componentName)
                {
                    case UriComponentsList.Base:
                        uri =$"{uri}{UriStructure.Base}{component.FinalCharacter}";
                        break;
                    case UriComponentsList.Character:
                        uri = $"{uri}{parsedCharacter}{component.FinalCharacter}";
                        break;
                    case UriComponentsList.ResourceClass:
                        uri = $"{uri}{parsedResourceClass}{component.FinalCharacter}";
                        break;
                    case UriComponentsList.Identifier:
                        containsKey = queryString.ContainsKey(UriComponentsList.Identifier);
                        if(!containsKey && component.Mandatory)
                        {
                            error = true;
                        }else if (containsKey)
                        {
                            string id = queryString[UriComponentsList.Identifier];
                            uri = $"{uri}{id}{component.FinalCharacter}";
                        }
                        break;
                    default:
                        containsKey = queryString.ContainsKey(componentName);
                        if (!containsKey && component.Mandatory)
                        {
                            error = true; 
                            errorMessage = $"{errorMessage} parameter {componentName} missing \n"; 
                        }
                        else if (containsKey)
                        {
                            string componentVariable = queryString[componentName];
                            uri = $"{uri}{componentVariable}{component.FinalCharacter}";
                        }
                        break;
                }
            }
            if (error)
            {
                throw new ParametersNotConfiguredException(errorMessage);
            }
            return uri;
        }

        ///<summary>
        ///devuelve el Character que usa una estructura Uri
        ///</summary>
        ///<param name="pUriStructureComponents">componentes de la estructura uri</param>
        private string ParserCharacter(List<Component> pUriStructureComponents)
        {
            string labelCharacter = null;
            string uriComponentValue = pUriStructureComponents.FirstOrDefault(component => component.UriComponent.Equals(UriComponentsList.Character)).UriComponentValue;
            string[] parameters = uriComponentValue.Split('@');
            if(parameters.Length == 2)
            {
                string character = parameters[1].ToLower();
                Characters characterObject = UriStructure.Characters.FirstOrDefault(charac => charac.Character.Equals(character));
                if(characterObject != null)
                {
                    labelCharacter = characterObject.LabelCharacter;
                }
            }  
            return labelCharacter;
        }

        ///<summary>
        ///devuelve un objeto ResourceClass a partir del nombre de resource class
        ///</summary>
        ///<param name="pResourceClass">nombre del objeto a devolver</param>
        private ResourcesClass ParserResourceClass(string pResourceClass)
        {
            ResourcesClass resourceClass = null;
            resourceClass = UriStructure.ResourcesClasses.FirstOrDefault(resource => resource.ResourceClass.Equals(pResourceClass));
            return resourceClass;
        }

        ///<summary>
        ///devuelve un objeto ResourceClass a partir del RDFType
        ///</summary>
        ///<param name="pRdfType">rdftype del objeto a devolver</param>
        private ResourcesClass ParserResourceClassRdfType(string pRdfType)
        {
            ResourcesClass resourceClass = null;
            resourceClass = UriStructure.ResourcesClasses.FirstOrDefault(resource => resource.RdfType.Equals(pRdfType));
            return resourceClass;
        }
    }
}
