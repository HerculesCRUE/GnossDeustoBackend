using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrisFactory.Models.ConfigEntities;

namespace UrisFactory.ModelExamples
{
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
                        LabelResourceClass = "researcher",
                        ResourceURI = "uriResourceStructure"
                    },
                    new ResourcesClass()
                    {
                        ResourceClass = "Activity",
                        LabelResourceClass = "activity",
                        ResourceURI = "uriActivityStructure"
                    }
                }
            };
        }
    }
}
