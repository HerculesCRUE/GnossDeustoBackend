using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.ViewModels;

namespace UrisFactory.ModelExamples
{
    public class UriStructureInfoRequest: IExamplesProvider<InfoUriStructure>
    {
        public InfoUriStructure GetExamples()
        {
            ResourcesClass item = new ResourcesClass()
            {
                ResourceClass = "Example",
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
