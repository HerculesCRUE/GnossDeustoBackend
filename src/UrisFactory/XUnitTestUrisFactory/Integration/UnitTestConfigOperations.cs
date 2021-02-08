// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test de integración del fichero de configuración
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using Xunit;

namespace XUnitTestUrisFactory.Integration
{
    public class UnitTestConfigOperations
    {
        public static bool IsDeleting { get; set; }

        private static string _uriBackUpConfig = "config/UrisConfig_copy.json";
        private static string _uriConfig = "config/UrisConfig.json";

        public void TestGetSchemaFileData()
        {
            ISchemaConfigOperations schemaConfigOperations = new SchemaConfigFileOperations(null);
            var bytesSchema = schemaConfigOperations.GetFileSchemaData();
            var bytesAsString = Encoding.UTF8.GetString(bytesSchema);
            UriStructureGeneral uriSchema = JsonConvert.DeserializeObject<UriStructureGeneral>(bytesAsString);
            bool correctGenerated = uriSchema != null && uriSchema.Base != null && uriSchema.Characters != null && uriSchema.ResourcesClasses != null && uriSchema.ResourcesClasses.Count > 0 && uriSchema.UriStructures != null && uriSchema.UriStructures.Count > 0;
            Assert.True(correctGenerated);
        }

        public void TestDeleteUriStructureOk()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                IsDeleting = true;
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                if (configJsonHandler.ExistUriStructure("uriPublicationStructure"))
                {
                    configJsonHandler.DeleteUriStructureInfo("uriPublicationStructure");
                    SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                    schemaConfigFileOperations.SaveConfigJson();
                    UriStructureGeneral uriSchema2 = configJsonHandler.GetUrisConfig();
                    RestoreBackUpConfig();
                    configJsonHandler.LoadConfigJson();
                    IsDeleting = false;
                    Assert.True(oldResourcesClassesCount == uriSchema2.ResourcesClasses.Count + 1 && oldUriStructuresCount == uriSchema2.UriStructures.Count + 1);
                }
                else
                {
                    IsDeleting = false;
                    Assert.True(false);
                }
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
                IsDeleting = false;
            }
        }

        public void TestDeleteUriStructureNoNameFoundError()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                CreateBackUpConfig();
                Assert.Throws<UriStructureConfiguredException>(() => configJsonHandler.DeleteUriStructureInfo("badName"));
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                schemaConfigFileOperations.SaveConfigJson();
                configJsonHandler.GetUrisConfig();
                RestoreBackUpConfig();
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        public void TestAddUriStructureOk()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                CreateBackUpConfig();
                UriStructureGeneral uriSchema = configJsonHandler.GetUrisConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "pipaon");
                int oldResourcesClassesCount = uriSchema.ResourcesClasses.Count;
                int oldUriStructuresCount = uriSchema.UriStructures.Count;
                configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass);
                SchemaConfigFileOperations schemaConfigFileOperations = new SchemaConfigFileOperations(configJsonHandler);
                schemaConfigFileOperations.SaveConfigJson();
                UriStructureGeneral uriSchema2 = configJsonHandler.GetUrisConfig();
                RestoreBackUpConfig();
                configJsonHandler.LoadConfigJson();
                Assert.True(oldResourcesClassesCount + 1 == uriSchema2.ResourcesClasses.Count && oldUriStructuresCount + 1 == uriSchema2.UriStructures.Count);
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        public void TestAddUriStructureFailMatchNames()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
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

        public void TestAddUriStructureFailUriStructureConfiguredException()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                CreateBackUpConfig();
                UriStructure newUriStructure = CreateUriStructureExample("uriResourceStructure");
                ResourcesClass newResourcesClass = CreateResourceClassExample("uriResourceStructure", "rsp", "");
                Assert.Throws<UriStructureConfiguredException>(() => configJsonHandler.AddUriStructureInfo(newUriStructure, newResourcesClass));
            }
            catch (Exception)
            {
                RestoreBackUpConfig();
            }
        }

        public void TestAddUriStructureFailUriStructureBadInfoException()
        {
            try
            {
                ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
                CreateBackUpConfig();
                UriStructure newUriStructure = CreateUriStructureExample("newUriExample");
                ResourcesClass newResourcesClass = CreateResourceClassExample("newUriExample", "rsp", "");
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
