using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using Xunit;

namespace XUnitTestUrisFactory
{
    public class UnitTestConfigOperations
    {
        private static string _uriBackUpConfig = "config/UrisConfig_copy.json";
        private static string _uriConfig = "config/UrisConfig.json";

        [Fact]
        public void TestGetSchemaFileData()
        {
            var bytesSchema = SchemaConfigFileOperations.GetFileSchemaData();
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
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = ConfigJsonHandler.GetUrisConfig();
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                if (ConfigJsonHandler.ExistUriStructure("uriPublicationStructure"))
                {
                    ConfigJsonHandler.DeleteUriStructureInfo("uriPublicationStructure");
                    SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
                    UriStructureGeneral uriSchema2 = ConfigJsonHandler.GetUrisConfig();
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
                CreateBackUpConfig();
                Assert.Throws<UriStructureConfiguredException>(() => ConfigJsonHandler.DeleteUriStructureInfo("badName"));
                SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
                ConfigJsonHandler.GetUrisConfig();
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
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = ConfigJsonHandler.GetUrisConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                ConfigJsonHandler.AddUriStructureInfo(newUriStructure,newResourcesClass);
                SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
                UriStructureGeneral uriSchema2 = ConfigJsonHandler.GetUrisConfig();
                RestoreBackUpConfig();
                Assert.Throws<UriStructureConfiguredException>(() => "");
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        [Fact]
        public void TestAddUriStructureFailUriStructureConfiguredException()
        {
            try
            {
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = ConfigJsonHandler.GetUrisConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExamp");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                ConfigJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass);
                SchemaConfigFileOperations.SaveConfigJsonInConfigFile();
                UriStructureGeneral uriSchema2 = ConfigJsonHandler.GetUrisConfig();
                RestoreBackUpConfig();
                Assert.True(oldResourcesClassesCount + 1 == uriSchema2.ResourcesClasses.Count && oldUriStructuresCount + 1 == uriSchema2.UriStructures.Count);
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
