using System;
using System.Collections.Generic;
using System.Linq;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
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

        public string GetURI(string resourceClass, Dictionary<string, string> queryString)
        {
            string uri = "";
            ResourcesClass resourceClassObject = ParserResourceClass(resourceClass);          

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

        private ResourcesClass ParserResourceClass(string pResourceClass)
        {
            ResourcesClass resourceClass = null;
            resourceClass = UriStructure.ResourcesClasses.FirstOrDefault(resource => resource.ResourceClass.Equals(pResourceClass));
            return resourceClass;
        }
    }
}
