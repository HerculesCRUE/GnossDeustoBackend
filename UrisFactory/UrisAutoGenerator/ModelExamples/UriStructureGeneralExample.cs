// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase de ejemplo para mostrar un ejemplo del fichero de configuración
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.ModelExamples
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase de ejemplo para mostrar un ejemplo del fichero de configuración
    ///</summary>
    public class UriStructureGeneralExample : IExamplesProvider<UriStructureGeneral>
    {
        public UriStructureGeneral GetExamples()
        {
            return new UriStructureGeneral()
            {
                Base = "http://data.um.es",
                Characters = new List<Characters>()
                {
                    new Characters()
                    {
                        Character = "resource",
                        LabelCharacter = "res"
                    }
                },
                UriStructures = new List<UriStructure>()
                {
                    new UriStructure()
                    {
                        Name = "uriResourceStructure",
                        Components = new List<Component>()
                        {
                            new Component()
                            {
                                UriComponent = "base",
                                UriComponentValue = "base",
                                UriComponentOrder = 1,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "character",
                                UriComponentValue = "@RESOURCE",
                                UriComponentOrder = 2,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "resourceClass",
                                UriComponentValue = "@RESOURCECLASS",
                                UriComponentOrder = 3,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "identifier",
                                UriComponentValue = "@ID",
                                UriComponentOrder = 4,
                                Mandatory = true,
                                FinalCharacter = ""
                            }
                        }
                    },
                    new UriStructure()
                    {
                        Name = "uriActivityStructure",
                        Components = new List<Component>()
                        {
                            new Component()
                            {
                                UriComponent = "base",
                                UriComponentValue = "base",
                                UriComponentOrder = 1,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "character",
                                UriComponentValue = "@RESOURCE",
                                UriComponentOrder = 2,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "resourceClass",
                                UriComponentValue = "@RESOURCECLASS",
                                UriComponentOrder = 3,
                                Mandatory = true,
                                FinalCharacter = "/"
                            },
                            new Component()
                            {
                                UriComponent = "identifier",
                                UriComponentValue = "@ID",
                                UriComponentOrder = 4,
                                Mandatory = true,
                                FinalCharacter = ""
                            }
                        }
                    }
                },
                ResourcesClasses = new List<ResourcesClass>()
                {
                    new ResourcesClass()
                    {
                        ResourceClass = "Researcher",
                        RdfType = "investigador",
                        LabelResourceClass = "researcher",
                        ResourceURI = "uriResourceStructure"
                    },
                    new ResourcesClass()
                    {
                        ResourceClass = "Activity",
                        RdfType = "actividad",
                        LabelResourceClass = "activity",
                        ResourceURI = "uriActivityStructure"
                    }
                }
            };
        }
    }
}
