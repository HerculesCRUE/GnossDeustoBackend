using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsRepositoriesConfig
    {
        [Fact]
        public void GetConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            List<RepositoryConfig> listaRepositorios = repositoriesConfigMockService.GetRepositoryConfigs();
            if (listaRepositorios.Count > 0)
            {
                RepositoryConfig repositoryConfig = listaRepositorios[0];
                RepositoryConfig repositoryConfigGetByID = repositoriesConfigMockService.GetRepositoryConfigById(repositoryConfig.RepositoryConfigID);
                Assert.True(repositoryConfig.Name.Equals(repositoryConfigGetByID.Name));
            }
            
        }

        [Fact]
        public void DeleteConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            RepositoryConfig repositoryConfig = repositoriesConfigMockService.GetRepositoryConfigs()[0];
            repositoriesConfigMockService.RemoveRepositoryConfig(repositoryConfig.RepositoryConfigID);
            repositoryConfig = repositoriesConfigMockService.GetRepositoryConfigById(repositoryConfig.RepositoryConfigID);
            Assert.Null(repositoryConfig);
        }

        [Fact]
        public void AddConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            RepositoryConfig repositoryConfigToAdd = new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "Un repositorio para configurarlos a todos",
                Url = "config\\repository",
                OauthToken = "12weq1"
            };
            Guid identifierAdded = repositoriesConfigMockService.AddRepositoryConfig(repositoryConfigToAdd);
            RepositoryConfig repositoryConfig = repositoriesConfigMockService.GetRepositoryConfigById(identifierAdded);
            Assert.True(repositoryConfigToAdd.Name.Equals(repositoryConfig.Name));
        }



    }
}
