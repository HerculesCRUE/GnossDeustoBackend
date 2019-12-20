using System;
using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class UriFormer
    {
        private static UriStructureGeneral _uristructure;

        private static UriStructureGeneral UriStructure
        {
            get
            {
                if (_uristructure == null){
                    _uristructure = ConfigJsonHandler.GetUriStructure();
                }
                return _uristructure;
                    
            }
            
        }

        public static string GetURI(string character, string resourceClass, Dictionary<string, string> queryString)
        {
            string uri = "";
            string parsedCharacter = ParserCharacter(character);
            ResourcesClass resourceClassObject = ParserResourceClass(resourceClass);

            if (resourceClassObject != null && !string.IsNullOrEmpty(parsedCharacter))
            {
                string parsedLabelResourceClass = resourceClassObject.LabelResourceClass;
                string resourceURL = resourceClassObject.ResourceURI;

                UriStructure urlStructure = UriStructure.UriStructures.FirstOrDefault(structure => structure.Name.Equals(resourceURL));

                if (urlStructure != null)
                {
                    uri = GetUriByStructure(urlStructure, parsedCharacter, parsedLabelResourceClass, queryString);
                }
                else
                {
                    throw new ParametersNotConfiguredException($"Structure {urlStructure} not configured");
                }
                return uri;
            }
            else
            {
                if (resourceClassObject == null && string.IsNullOrEmpty(parsedCharacter))
                {
                    throw new ParametersNotConfiguredException("resource class and resource not configured");
                }
                else if (resourceClassObject == null)
                {
                    throw new ParametersNotConfiguredException("resource class not configured");
                }
                else
                {
                    throw new ParametersNotConfiguredException("resource not configured");
                }
            }
        }

        private static string GetUriByStructure(UriStructure urlStructure, string parsedCharacter, string parsedResourceClass, Dictionary<string, string> queryString)
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

        private static string ParserCharacter(string pCharacter)
        {
            string character = UriStructure.Characters.Where(charact => charact.Character.Equals(pCharacter)).Select(charact => charact.LabelCharacter).FirstOrDefault();
            return character;
        }

        private static ResourcesClass ParserResourceClass(string pResourceClass)
        {
            ResourcesClass resourceClass = null;
            resourceClass = UriStructure.ResourcesClasses.FirstOrDefault(resource => resource.ResourceClass.Equals(pResourceClass));
            return resourceClass;
        }
    }
}
