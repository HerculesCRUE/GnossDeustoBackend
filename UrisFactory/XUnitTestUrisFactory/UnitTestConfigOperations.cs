using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using Xunit;
using XUnitTestUrisFactory.Properties;
using XUnitTestUrisFactory.services;

namespace XUnitTestUrisFactory
{
    public class UnitTestConfigOperations
    {
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
            if (configJsonHandler.ExistUriStructure("uriPublicationStructure"))
            {
                configJsonHandler.DeleteUriStructureInfo("uriPublicationStructure");
                ISchemaConfigOperations schemaConfigOperations = new SchemaConfigMemoryOperations(configJsonHandler);
                schemaConfigOperations.SaveConfigJson();
                UriStructureGeneral uriSchema2 = configJsonHandler.GetUrisConfig();
                Assert.True(oldResourcesClassesCount == uriSchema2.ResourcesClasses.Count + 1 && oldUriStructuresCount == uriSchema2.UriStructures.Count + 1);
            }
            else
            {
                Assert.True(false);
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
    }
}
