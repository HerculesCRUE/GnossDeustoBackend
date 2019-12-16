using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.Models.Services
{
    public class UriFormer
    {
        private static UriStructure _uristructure;

        private static UriStructure UriStructure
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

            if (resourceClassObject != null)
            {
                string parsedLabelResourceClass = resourceClassObject.LabelResourceClass;
                string resourceURL = resourceClassObject.ResourceURL;

                UrlStructure urlStructure = UriStructure.UrlStructures.FirstOrDefault(structure => structure.Name.Equals(resourceURL));

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
                if (resourceClassObject == null)
                {
                    throw new ParametersNotConfiguredException("resource class not configured");
                }
            }
            return uri;
        }

        private static string GetUriByStructure(UrlStructure urlStructure, string parsedCharacter, string parsedResourceClass, Dictionary<string, string> queryString)
        {
            string uri = "";
            bool error = false;
            bool containsKey = false;
            foreach (Component component in urlStructure.Components.OrderBy(structure => structure.UrlComponentOrder))
            {
                string componentName = component.UrlComponent;
                switch (componentName)
                {
                    case UrlComponentsList.Base:
                        uri += UriStructure.Base + component.FinalCharacter;
                        break;
                    case UrlComponentsList.Character:
                        uri += parsedCharacter + component.FinalCharacter;
                        break;
                    case UrlComponentsList.ResourceClass:
                        uri += parsedResourceClass + component.FinalCharacter;
                        break;
                    case UrlComponentsList.Identifier:
                        containsKey = queryString.ContainsKey(UrlComponentsList.Identifier);
                        if(!containsKey && component.Mandatory)
                        {
                            error = true;
                        }else if (containsKey)
                        {
                            string id = queryString[UrlComponentsList.Identifier];
                            uri += id + component.FinalCharacter;
                        }
                        break;
                    default:
                        containsKey = queryString.ContainsKey(componentName);
                        if (!containsKey && component.Mandatory)
                        {
                            error = true;
                        }
                        else if (containsKey)
                        {
                            string componentVariable = queryString[componentName];
                            uri += componentVariable + component.FinalCharacter;
                        }
                        break;
                }
            }
            if (error)
            {
                throw new ParametersNotConfiguredException("Parameters missing");
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
