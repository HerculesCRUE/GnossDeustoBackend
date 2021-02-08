using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using Xunit;
namespace XUnitTestIntegracion
{
    public class UnitTest1
    {
        private static string _uriBackUpConfig = "config/UrisConfig_copy.json";
        private static string _uriConfig = "config/UrisConfig.json";

        [Fact]
        public void TestGetSchemaFileData()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
            var bytesSchema = schemaConfigFileOperations.GetFileSchemaData();
            var bytesAsString = Encoding.UTF8.GetString(bytesSchema);
            UriStructureGeneral uriSchema = JsonConvert.DeserializeObject<UriStructureGeneral>(bytesAsString);
            bool correctGenerated = uriSchema != null && uriSchema.Base != null && uriSchema.Characters != null && uriSchema.ResourcesClasses != null && uriSchema.ResourcesClasses.Count > 0 && uriSchema.UriStructures != null && uriSchema.UriStructures.Count > 0;
            Assert.True(correctGenerated);
        }

        [Fact]
        public void TestDeleteUriStructureOk()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                if (configJsonHandler.ExistUriStructure("uriPublicationStructure"))
                {
                    configJsonHandler.DeleteUriStructureInfo("uriPublicationStructure");
                    schemaConfigFileOperations.SaveConfigJson();
                    ConfigJsonHandler configJsonHandler2 = new ConfigJsonHandler();
                    UriStructureGeneral uriSchema2 = configJsonHandler2.GetUrisConfig();
                    RestoreBackUpConfig();
                    Assert.True(oldResourcesClassesCount == uriSchema2.ResourcesClasses.Count + 1 && oldUriStructuresCount == uriSchema2.UriStructures.Count + 1);
                }
                else
                {
                    Assert.True(false);
                }
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        [Fact]
        public void TestDeleteUriStructureNoNameFoundError()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                CreateBackUpConfig();
                Assert.Throws<UriStructureConfiguredException>(() => configJsonHandler.DeleteUriStructureInfo("badName"));
                schemaConfigFileOperations.SaveConfigJson();
                configJsonHandler.GetUrisConfig();
                RestoreBackUpConfig();
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        [Fact]
        public void TestAddUriStructureOk()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass);
                schemaConfigFileOperations.SaveConfigJson();
                ConfigJsonHandler configJsonHandler2 = new ConfigJsonHandler();
                UriStructureGeneral uriSchema2 = configJsonHandler2.GetUrisConfig();
                RestoreBackUpConfig();
                configJsonHandler2.LoadConfigJson();
                Assert.True(oldResourcesClassesCount + 1 == uriSchema2.ResourcesClasses.Count && oldUriStructuresCount + 1 == uriSchema2.UriStructures.Count);
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        [Fact]
        public void TestAddUriStructureFailMatchNames()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                CreateBackUpConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExamp");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
                Assert.Throws<UriStructureBadInfoException>(() => configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass));
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        private void CreateBackUpConfig()
        {
            File.Copy(_uriConfig, _uriBackUpConfig);
        }

        private void RestoreBackUpConfig()
        {
            File.Delete(_uriConfig);
            File.Move(_uriBackUpConfig, _uriConfig);
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

