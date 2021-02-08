// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase de ejemplo para mostrar un ejemplo de estructura uri
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.ViewModels;

namespace UrisFactory.ModelExamples
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase de ejemplo para mostrar un ejemplo de estructura uri
    ///</summary>
    public class UriStructureInfoRequest: IExamplesProvider<InfoUriStructure>
    {
        public InfoUriStructure GetExamples()
        {
            ResourcesClass item = new ResourcesClass()
            {
                ResourceClass = "Example",
                RdfType = "Ejemplo",
                LabelResourceClass = "example",
                ResourceURI = "uriExampleStructure"
            };
            List<ResourcesClass> list = new List<ResourcesClass>();
            list.Add(item);
            return new InfoUriStructure
            {
                
                ResourcesClass = list,
                UriStructure = new UriStructure
                {
                    Name = "uriExampleStructure",
                    Components = new List<Component>
                    {
                        new Component
                        {
                            UriComponent = "base",
                            UriComponentValue = "base",
                            UriComponentOrder = 1,
                            Mandatory = true,
                            FinalCharacter = "/"
                        },
                        new Component
                        {
                            UriComponent = "character",
                            UriComponentValue = "character@RESOURCE",
                            UriComponentOrder = 2,
                            Mandatory = true,
                            FinalCharacter = "/"
                        },
                        new Component
                        {
                            UriComponent = "resourceClass",
                            UriComponentValue = "resourceClass@RESOURCECLASS",
                            UriComponentOrder = 3,
                            Mandatory = true,
                            FinalCharacter = "/"
                        },
                        new Component
                        {
                            UriComponent = "identifier",
                            UriComponentValue = "@ID",
                            UriComponentOrder = 4,
                            Mandatory = true,
                            FinalCharacter = ""
                        }
                    }

                }
            };
        }
    }
}
