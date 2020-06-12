// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test unitario de reposotorios
using API_CARGA.Controllers;
using API_CARGA.Models.Entities;
using API_CARGA.Models.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestAPI_CARGA
{
    public class UnitTestOperationsRepository
    {
        [Fact]
        public void GetRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            repositoryController repositoryController = new repositoryController(repositoriesConfigMockService);
            List<RepositoryConfig> listaRepositorios = (List<RepositoryConfig>)(((OkObjectResult)repositoryController.GetRepository()).Value);
            Assert.True(listaRepositorios.Count > 0);            
        }

        [Fact]
        public void GetRepositoryByID()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            repositoryController repositoryController = new repositoryController(repositoriesConfigMockService);
            List<RepositoryConfig> listaRepositorios = (List<RepositoryConfig>)(((OkObjectResult)repositoryController.GetRepository()).Value);
            if (listaRepositorios.Count > 0)
            {
                RepositoryConfig repositoryConfig = listaRepositorios[0];
                RepositoryConfig repositoryConfigGetByID = (RepositoryConfig)(((OkObjectResult)repositoryController.GetRepository(repositoryConfig.RepositoryConfigID)).Value);
                Assert.True(repositoryConfig.Name.Equals(repositoryConfigGetByID.Name));
            }
        }


        [Fact]
        public void AddConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            repositoryController repositoryController = new repositoryController(repositoriesConfigMockService);
            RepositoryConfig repositoryConfigToAdd = new RepositoryConfig
            {
                Name = "Un repositorio para configurarlos a todos",
                Url = "config\\repository",
                OauthToken = "12weq1"
            };
            Guid identifierAdded = (Guid)(((OkObjectResult)repositoryController.AddConfigRepository(repositoryConfigToAdd)).Value);
            RepositoryConfig repositoryConfig = (RepositoryConfig)(((OkObjectResult)repositoryController.GetRepository(identifierAdded)).Value);
            Assert.True(repositoryConfigToAdd.Name.Equals(repositoryConfig.Name));
        }


        [Fact]
        public void DeleteConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            repositoryController repositoryController = new repositoryController(repositoriesConfigMockService);
            RepositoryConfig repositoryConfig = ((List<RepositoryConfig>)(((OkObjectResult)repositoryController.GetRepository()).Value))[0];
            repositoryController.DeleteRepository(repositoryConfig.RepositoryConfigID);
            repositoryConfig = (RepositoryConfig)(((OkObjectResult)repositoryController.GetRepository(repositoryConfig.RepositoryConfigID)).Value);
            Assert.Null(repositoryConfig);
        }

        [Fact]
        public void ModifyConfigRepository()
        {
            RepositoriesConfigMockService repositoriesConfigMockService = new RepositoriesConfigMockService();
            repositoryController repositoryController = new repositoryController(repositoriesConfigMockService);
            RepositoryConfig repositoryConfig = ((List<RepositoryConfig>)(((OkObjectResult)repositoryController.GetRepository()).Value))[0];
            Random random = new Random();
            string newName = "updatedRepository_" + random.NextDouble();
            repositoryConfig.Name = newName;
            repositoryController.ModifyRepositoryConfig(repositoryConfig);
            RepositoryConfig updatedrepositoryConfig = (RepositoryConfig)(((OkObjectResult)repositoryController.GetRepository(repositoryConfig.RepositoryConfigID)).Value);
            Assert.True(updatedrepositoryConfig.Name.Equals(newName));
        }

    }
}
