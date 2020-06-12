// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test unitario del fichero de configuración
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Controllers;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using UrisFactory.ViewModels;
using Xunit;
using XUnitTestUrisFactory.Properties;
using XUnitTestUrisFactory.services;

namespace XUnitTestUrisFactory
{
    public class UnitTestConfigOperations
    {
        [Fact]
        public void TestGetSchemaController()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
                SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
                var result = schemaController.GetSchema();
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestFormedJson()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(jsonConfig);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "config.json");
            var result = schemaController.ReplaceSchemaConfig(file);
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
            
            }
        [Fact]
        public void TestBadFormedJson()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(jsonConfigBad);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "config.json");
            var result = schemaController.ReplaceSchemaConfig(file);
            if (result is BadRequestObjectResult)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestGetUriController()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            var result = schemaController.GetUriStructureInfo("uriResourceStructure");
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestGetUriFailController()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            var result = schemaController.GetUriStructureInfo("uriResourceStructur");
            if (result is BadRequestObjectResult)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestGetSchemaFileData()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            var bytesSchema = schemaConfigOperations.GetFileSchemaData();
            var bytesAsString = Encoding.UTF8.GetString(bytesSchema);
            UriStructureGeneral uriSchema = JsonConvert.DeserializeObject<UriStructureGeneral>(bytesAsString);
            bool correctGenerated = uriSchema != null && uriSchema.Base != null && uriSchema.Characters != null && uriSchema.ResourcesClasses != null && uriSchema.ResourcesClasses.Count > 0 && uriSchema.UriStructures != null && uriSchema.UriStructures.Count > 0;
            Assert.True(correctGenerated);
        }

        [Fact]
        public void TestDeleteUriStructureOk()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
            int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
            int oldUriStructuresCount = uriSchema.UriStructures.Count;
            if (configJsonHandler.ExistUriStructure("uriResourceStructure"))
            {
                configJsonHandler.DeleteUriStructureInfo("uriResourceStructure");
                ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
                schemaConfigOperations.SaveConfigJson();
                UriStructureGeneral uriSchema2 = configJsonHandler.GetUrisConfig();
                Assert.True(oldResourcesClassesCount > uriSchema2.ResourcesClasses.Count && oldUriStructuresCount > uriSchema2.UriStructures.Count);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestDeleteUriStructureOkController()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            var result = schemaController.DeleteUriStructure("uriResourceStructure");
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestDeleteUriStructureNoNameFoundError()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            Assert.Throws<UriStructureConfiguredException>(() => configJsonHandler.DeleteUriStructureInfo("badName"));
        }

        [Fact]
        public void TestAddUriStructureOk()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
            UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
            ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
            int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
            int oldUriStructuresCount = uriSchema.UriStructures.Count;
            configJsonHandler.AddUriStructureInfo(newUriStructure,newResourcesClass);
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            schemaConfigOperations.SaveConfigJson();
            UriStructureGeneral uriSchema2 = configJsonHandler.GetUrisConfig();
            Assert.True(oldResourcesClassesCount + 1 == uriSchema2.ResourcesClasses.Count && oldUriStructuresCount + 1 == uriSchema2.UriStructures.Count);
        }

        [Fact]
        public void TestAddUriStructureOkController()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
            ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
            List<ResourcesClass> lista = new List<ResourcesClass>();
            lista.Add(newResourcesClass);
            InfoUriStructure structure = new InfoUriStructure();
            structure.ResourcesClass = lista;
            structure.UriStructure = newUriStructure;
            var result = schemaController.AddUriStructure(structure);
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestAddUriStructureFailMatchNamesController()
        {

            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
            SchemaController schemaController = new SchemaController(configJsonHandler, schemaConfigOperations);
            UriStructure newUriStructure = CreateUriStructureExample("newUriExamp");
            ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
            InfoUriStructure structure = new InfoUriStructure();
            List<ResourcesClass> lista = new List<ResourcesClass>();
            lista.Add(newResourcesClass);
            structure.ResourcesClass = lista;
            structure.UriStructure = newUriStructure;
            var result = schemaController.AddUriStructure(structure);
            if (result is BadRequestObjectResult)
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestAddUriStructureFailMatchNames()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriStructure newUriStructure = CreateUriStructureExample("newUriExamp");
            ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
            Assert.Throws<UriStructureBadInfoException>(() => configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass));
        }

        [Fact]
        public void TestAddUriStructureFailUriStructureConfiguredException()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriStructure newUriStructure = CreateUriStructureExample("uriResourceStructure");
            ResourcesClass newResourcesClass = CreateResourceClassExample("uriResourceStructure", "rsp", "");
            Assert.Throws<UriStructureConfiguredException>(() => configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass));
        }

        [Fact]
        public void TestAddUriStructureFailUriStructureBadInfoException()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
            ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "");
            Assert.Throws<UriStructureBadInfoException>(() => configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass));
        }


        private UriStructure CreateUriStructureExample(string name)
        {
            UriStructure newUriStructure = new UriStructure()
            {
                Name = name,
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
            };
            return newUriStructure;
        }

        private ResourcesClass CreateResourceClassExample(string name, string resourceClass, string labelRsourceClass)
        {
            ResourcesClass resourcesClass = new ResourcesClass()
            {
                ResourceURI = name,
                LabelResourceClass = labelRsourceClass,
                ResourceClass = resourceClass
            };
            return resourcesClass;
        }

        private string jsonConfig
        {
            get
            {
                return "{\r\n  \"Base\": \"http://graph.um.es\",\r\n  \"Characters\": [\r\n    {\r\n      \"Character\": \"resource\",\r\n      \"LabelCharacter\": \"res\"\r\n    },\r\n    {\r\n      \"Character\": \"kos\",\r\n      \"LabelCharacter\": \"kos\"\r\n    }\r\n  ],\r\n  \"UriStructures\": [\r\n    {\r\n      \"Name\": \"uriResourceStructure\",\r\n      \"Components\": [\r\n        {\r\n          \"UriComponent\": \"base\",\r\n          \"UriComponentValue\": \"base\",\r\n          \"UriComponentOrder\": 1,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"character\",\r\n          \"UriComponentValue\": \"character@RESOURCE\",\r\n          \"UriComponentOrder\": 2,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"resourceClass\",\r\n          \"UriComponentValue\": \"resourceClass@RESOURCECLASS\",\r\n          \"UriComponentOrder\": 3,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"identifier\",\r\n          \"UriComponentValue\": \"@ID\",\r\n          \"UriComponentOrder\": 4,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"Name\": \"uriKosStructure\",\r\n      \"Components\": [\r\n        {\r\n          \"UriComponent\": \"base\",\r\n          \"UriComponentValue\": \"base\",\r\n          \"UriComponentOrder\": 1,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"character\",\r\n          \"UriComponentValue\": \"character@KOS\",\r\n          \"UriComponentOrder\": 2,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"resourceClass\",\r\n          \"UriComponentValue\": \"resourceClass@RESOURCECLASS\",\r\n          \"UriComponentOrder\": 3,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"identifier\",\r\n          \"UriComponentValue\": \"@ID\",\r\n          \"UriComponentOrder\": 4,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"\"\r\n        }\r\n      ]\r\n    }\r\n  ],\r\n  \"ResourcesClasses\": [\r\n    {\r\n      \"ResourceClass\": \"Publication\",\r\n      \"LabelResourceClass\": \"publicacion\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Activity\",\r\n      \"LabelResourceClass\": \"activity\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Funding\",\r\n      \"LabelResourceClass\": \"funding\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingProgram\",\r\n      \"LabelResourceClass\": \"funding-program\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"GeographicalScope\",\r\n      \"LabelResourceClass\": \"geographical-scope\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Metric\",\r\n      \"LabelResourceClass\": \"metric\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AcademicDegree\",\r\n      \"LabelResourceClass\": \"academic-degree\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AwardedDegree\",\r\n      \"LabelResourceClass\": \"awarded-degree\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Funder\",\r\n      \"LabelResourceClass\": \"funder\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"GeonamesFeature\",\r\n      \"LabelResourceClass\": \"geonames-feature\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Concept\",\r\n      \"LabelResourceClass\": \"concept\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"KnowledgeArea\",\r\n      \"LabelResourceClass\": \"knowledge-area\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchLine\",\r\n      \"LabelResourceClass\": \"research-line\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Keyword\",\r\n      \"LabelResourceClass\": \"keyword\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Project\",\r\n      \"LabelResourceClass\": \"project\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchObject\",\r\n      \"LabelResourceClass\": \"research-object\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Collection\",\r\n      \"LabelResourceClass\": \"collection\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Periodical\",\r\n      \"LabelResourceClass\": \"periodical\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Journal\",\r\n      \"LabelResourceClass\": \"journal\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Magazine\",\r\n      \"LabelResourceClass\": \"magazine\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Document\",\r\n      \"LabelResourceClass\": \"document\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Article\",\r\n      \"LabelResourceClass\": \"article\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ConferencePaper\",\r\n      \"LabelResourceClass\": \"conference-paper\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"EditorialArticle\",\r\n      \"LabelResourceClass\": \"editorial-article\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Book\",\r\n      \"LabelResourceClass\": \"book\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Proceedings\",\r\n      \"LabelResourceClass\": \"proceedings\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"DocumentPart\",\r\n      \"LabelResourceClass\": \"document-part\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"BookSection\",\r\n      \"LabelResourceClass\": \"book-section\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Chapter\",\r\n      \"LabelResourceClass\": \"chapter\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Patent\",\r\n      \"LabelResourceClass\": \"patent\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Report\",\r\n      \"LabelResourceClass\": \"report\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Thesis\",\r\n      \"LabelResourceClass\": \"thesis\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Webpage\",\r\n      \"LabelResourceClass\": \"webpage\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ConferencePoster\",\r\n      \"LabelResourceClass\": \"conference-poster\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Role\",\r\n      \"LabelResourceClass\": \"Role\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Example\",\r\n      \"LabelResourceClass\": \"example\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingAmount\",\r\n      \"LabelResourceClass\": \"fundingamount\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingSource\",\r\n      \"LabelResourceClass\": \"fundingsource\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ProjectExpense\",\r\n      \"LabelResourceClass\": \"projectexpense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"DateTimeInterval\",\r\n      \"LabelResourceClass\": \"datetimeinterval\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ProjectContract\",\r\n      \"LabelResourceClass\": \"projectcontract\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"InvestigatorRole\",\r\n      \"LabelResourceClass\": \"investigatorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Seq\",\r\n      \"LabelResourceClass\": \"seq\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Exhibit\",\r\n      \"LabelResourceClass\": \"exhibit\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Person\",\r\n      \"LabelResourceClass\": \"person\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Position\",\r\n      \"LabelResourceClass\": \"position\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Organization\",\r\n      \"LabelResourceClass\": \"organization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"University\",\r\n      \"LabelResourceClass\": \"university\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PhDThesis\",\r\n      \"LabelResourceClass\": \"phdthesis\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PrincipalInvestigatorRole\",\r\n      \"LabelResourceClass\": \"principalinvestigatorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"LeaderRole\",\r\n      \"LabelResourceClass\": \"leaderrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Internship\",\r\n      \"LabelResourceClass\": \"internship\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AdministratorRole\",\r\n      \"LabelResourceClass\": \"administratorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"StaffRole\",\r\n      \"LabelResourceClass\": \"staffrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ExternalMemberRole\",\r\n      \"LabelResourceClass\": \"externalmemberrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"MemberRole\",\r\n      \"LabelResourceClass\": \"memberrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PresenterRole\",\r\n      \"LabelResourceClass\": \"presenterrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardAddress\",\r\n      \"LabelResourceClass\": \"vcardaddress\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardAgent\",\r\n      \"LabelResourceClass\": \"vcardagent\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardContact\",\r\n      \"LabelResourceClass\": \"vcardcontact\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardDate\",\r\n      \"LabelResourceClass\": \"vcarddate\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardEmail\",\r\n      \"LabelResourceClass\": \"vcardmail\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardFax\",\r\n      \"LabelResourceClass\": \"vcardfax\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardGroup\",\r\n      \"LabelResourceClass\": \"vcardgroup\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardIndividual\",\r\n      \"LabelResourceClass\": \"vcardindividual\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardKind\",\r\n      \"LabelResourceClass\": \"vcardkind\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardLabel\",\r\n      \"LabelResourceClass\": \"vcardlabel\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardLocation\",\r\n      \"LabelResourceClass\": \"vcardlocation\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardName\",\r\n      \"LabelResourceClass\": \"vcardname\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardOrganization\",\r\n      \"LabelResourceClass\": \"vcardorganization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardPhone\",\r\n      \"LabelResourceClass\": \"vcardphone\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardPostal\",\r\n      \"LabelResourceClass\": \"vcardpostal\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardTelephone\",\r\n      \"LabelResourceClass\": \"vcardtelephone\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardURL\",\r\n      \"LabelResourceClass\": \"vcardurl\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchGroup\",\r\n      \"LabelResourceClass\": \"researchgroup\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Expense\",\r\n      \"LabelResourceClass\": \"expense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PhDSupervisingRelationship\",\r\n      \"LabelResourceClass\": \"phdsupervisingrelationship\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"SuperviseeRole\",\r\n      \"LabelResourceClass\": \"superviseerole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"SupervisorRole\",\r\n      \"LabelResourceClass\": \"supervisorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Conference\",\r\n      \"LabelResourceClass\": \"conference\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchObjectExpense\",\r\n      \"LabelResourceClass\": \"researchobjectexpense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"OrganizerRole\",\r\n      \"LabelResourceClass\": \"organizerrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"EditedBook\",\r\n      \"LabelResourceClass\": \"editedbook\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    }\r\n\t,\r\n    {\r\n      \"ResourceClass\": \"FundingOrganization\",\r\n      \"LabelResourceClass\": \"fundingorganization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Contract\",\r\n      \"LabelResourceClass\": \"contract\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    }\r\n\t\r\n  ]\r\n}\r\n";
            }
        }

        private string jsonConfigBad
        {
            get
            {
                return "{\r\n  \"Characters\": [\r\n    {\r\n      \"Character\": \"resource\",\r\n      \"LabelCharacter\": \"res\"\r\n    },\r\n    {\r\n      \"Character\": \"kos\",\r\n      \"LabelCharacter\": \"kos\"\r\n    }\r\n  ],\r\n  \"UriStructures\": [\r\n    {\r\n      \"Name\": \"uriResourceStructure\",\r\n      \"Components\": [\r\n        {\r\n          \"UriComponent\": \"base\",\r\n          \"UriComponentValue\": \"base\",\r\n          \"UriComponentOrder\": 1,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"character\",\r\n          \"UriComponentValue\": \"character@RESOURCE\",\r\n          \"UriComponentOrder\": 2,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"resourceClass\",\r\n          \"UriComponentValue\": \"resourceClass@RESOURCECLASS\",\r\n          \"UriComponentOrder\": 3,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"identifier\",\r\n          \"UriComponentValue\": \"@ID\",\r\n          \"UriComponentOrder\": 4,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"Name\": \"uriKosStructure\",\r\n      \"Components\": [\r\n        {\r\n          \"UriComponent\": \"base\",\r\n          \"UriComponentValue\": \"base\",\r\n          \"UriComponentOrder\": 1,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"character\",\r\n          \"UriComponentValue\": \"character@KOS\",\r\n          \"UriComponentOrder\": 2,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"resourceClass\",\r\n          \"UriComponentValue\": \"resourceClass@RESOURCECLASS\",\r\n          \"UriComponentOrder\": 3,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"/\"\r\n        },\r\n        {\r\n          \"UriComponent\": \"identifier\",\r\n          \"UriComponentValue\": \"@ID\",\r\n          \"UriComponentOrder\": 4,\r\n          \"Mandatory\": true,\r\n          \"FinalCharacter\": \"\"\r\n        }\r\n      ]\r\n    }\r\n  ],\r\n  \"ResourcesClasses\": [\r\n    {\r\n      \"ResourceClass\": \"Publication\",\r\n      \"LabelResourceClass\": \"publicacion\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Activity\",\r\n      \"LabelResourceClass\": \"activity\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Funding\",\r\n      \"LabelResourceClass\": \"funding\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingProgram\",\r\n      \"LabelResourceClass\": \"funding-program\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"GeographicalScope\",\r\n      \"LabelResourceClass\": \"geographical-scope\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Metric\",\r\n      \"LabelResourceClass\": \"metric\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AcademicDegree\",\r\n      \"LabelResourceClass\": \"academic-degree\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AwardedDegree\",\r\n      \"LabelResourceClass\": \"awarded-degree\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Funder\",\r\n      \"LabelResourceClass\": \"funder\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"GeonamesFeature\",\r\n      \"LabelResourceClass\": \"geonames-feature\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Concept\",\r\n      \"LabelResourceClass\": \"concept\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"KnowledgeArea\",\r\n      \"LabelResourceClass\": \"knowledge-area\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchLine\",\r\n      \"LabelResourceClass\": \"research-line\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Keyword\",\r\n      \"LabelResourceClass\": \"keyword\",\r\n      \"ResourceURI\": \"uriKosStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Project\",\r\n      \"LabelResourceClass\": \"project\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchObject\",\r\n      \"LabelResourceClass\": \"research-object\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Collection\",\r\n      \"LabelResourceClass\": \"collection\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Periodical\",\r\n      \"LabelResourceClass\": \"periodical\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Journal\",\r\n      \"LabelResourceClass\": \"journal\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Magazine\",\r\n      \"LabelResourceClass\": \"magazine\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Document\",\r\n      \"LabelResourceClass\": \"document\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Article\",\r\n      \"LabelResourceClass\": \"article\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ConferencePaper\",\r\n      \"LabelResourceClass\": \"conference-paper\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"EditorialArticle\",\r\n      \"LabelResourceClass\": \"editorial-article\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Book\",\r\n      \"LabelResourceClass\": \"book\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Proceedings\",\r\n      \"LabelResourceClass\": \"proceedings\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"DocumentPart\",\r\n      \"LabelResourceClass\": \"document-part\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"BookSection\",\r\n      \"LabelResourceClass\": \"book-section\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Chapter\",\r\n      \"LabelResourceClass\": \"chapter\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Patent\",\r\n      \"LabelResourceClass\": \"patent\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Report\",\r\n      \"LabelResourceClass\": \"report\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Thesis\",\r\n      \"LabelResourceClass\": \"thesis\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Webpage\",\r\n      \"LabelResourceClass\": \"webpage\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ConferencePoster\",\r\n      \"LabelResourceClass\": \"conference-poster\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Role\",\r\n      \"LabelResourceClass\": \"Role\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Example\",\r\n      \"LabelResourceClass\": \"example\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingAmount\",\r\n      \"LabelResourceClass\": \"fundingamount\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"FundingSource\",\r\n      \"LabelResourceClass\": \"fundingsource\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ProjectExpense\",\r\n      \"LabelResourceClass\": \"projectexpense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"DateTimeInterval\",\r\n      \"LabelResourceClass\": \"datetimeinterval\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ProjectContract\",\r\n      \"LabelResourceClass\": \"projectcontract\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"InvestigatorRole\",\r\n      \"LabelResourceClass\": \"investigatorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Seq\",\r\n      \"LabelResourceClass\": \"seq\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Exhibit\",\r\n      \"LabelResourceClass\": \"exhibit\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Person\",\r\n      \"LabelResourceClass\": \"person\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Position\",\r\n      \"LabelResourceClass\": \"position\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Organization\",\r\n      \"LabelResourceClass\": \"organization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"University\",\r\n      \"LabelResourceClass\": \"university\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PhDThesis\",\r\n      \"LabelResourceClass\": \"phdthesis\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PrincipalInvestigatorRole\",\r\n      \"LabelResourceClass\": \"principalinvestigatorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"LeaderRole\",\r\n      \"LabelResourceClass\": \"leaderrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Internship\",\r\n      \"LabelResourceClass\": \"internship\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"AdministratorRole\",\r\n      \"LabelResourceClass\": \"administratorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"StaffRole\",\r\n      \"LabelResourceClass\": \"staffrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ExternalMemberRole\",\r\n      \"LabelResourceClass\": \"externalmemberrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"MemberRole\",\r\n      \"LabelResourceClass\": \"memberrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PresenterRole\",\r\n      \"LabelResourceClass\": \"presenterrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardAddress\",\r\n      \"LabelResourceClass\": \"vcardaddress\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardAgent\",\r\n      \"LabelResourceClass\": \"vcardagent\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardContact\",\r\n      \"LabelResourceClass\": \"vcardcontact\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardDate\",\r\n      \"LabelResourceClass\": \"vcarddate\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardEmail\",\r\n      \"LabelResourceClass\": \"vcardmail\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardFax\",\r\n      \"LabelResourceClass\": \"vcardfax\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardGroup\",\r\n      \"LabelResourceClass\": \"vcardgroup\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardIndividual\",\r\n      \"LabelResourceClass\": \"vcardindividual\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardKind\",\r\n      \"LabelResourceClass\": \"vcardkind\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardLabel\",\r\n      \"LabelResourceClass\": \"vcardlabel\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardLocation\",\r\n      \"LabelResourceClass\": \"vcardlocation\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardName\",\r\n      \"LabelResourceClass\": \"vcardname\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardOrganization\",\r\n      \"LabelResourceClass\": \"vcardorganization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardPhone\",\r\n      \"LabelResourceClass\": \"vcardphone\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardPostal\",\r\n      \"LabelResourceClass\": \"vcardpostal\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardTelephone\",\r\n      \"LabelResourceClass\": \"vcardtelephone\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"VcardURL\",\r\n      \"LabelResourceClass\": \"vcardurl\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchGroup\",\r\n      \"LabelResourceClass\": \"researchgroup\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Expense\",\r\n      \"LabelResourceClass\": \"expense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"PhDSupervisingRelationship\",\r\n      \"LabelResourceClass\": \"phdsupervisingrelationship\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"SuperviseeRole\",\r\n      \"LabelResourceClass\": \"superviseerole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"SupervisorRole\",\r\n      \"LabelResourceClass\": \"supervisorrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Conference\",\r\n      \"LabelResourceClass\": \"conference\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"ResearchObjectExpense\",\r\n      \"LabelResourceClass\": \"researchobjectexpense\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"OrganizerRole\",\r\n      \"LabelResourceClass\": \"organizerrole\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"EditedBook\",\r\n      \"LabelResourceClass\": \"editedbook\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    }\r\n\t,\r\n    {\r\n      \"ResourceClass\": \"FundingOrganization\",\r\n      \"LabelResourceClass\": \"fundingorganization\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    },\r\n    {\r\n      \"ResourceClass\": \"Contract\",\r\n      \"LabelResourceClass\": \"contract\",\r\n      \"ResourceURI\": \"uriResourceStructure\"\r\n    }\r\n\t\r\n  ]\r\n}\r\n";
            }
        }
    }
}
