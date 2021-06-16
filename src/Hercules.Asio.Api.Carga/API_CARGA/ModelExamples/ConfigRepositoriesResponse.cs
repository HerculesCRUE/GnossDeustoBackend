// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve para mostrar un ejemplo de respuesta de una lista de repositorios 
using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un ejemplo de respuesta de una lista de repositorios 
    ///</summary>
    ///
    [ExcludeFromCodeCoverage]
    public class ConfigRepositoriesResponse : IExamplesProvider<List<RepositoryConfig>>
    {
        public List<RepositoryConfig> GetExamples()
        {
            List<RepositoryConfig> repositoryConfigs = new List<RepositoryConfig>();
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_1",
                Url = "https://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "12weq1"
            });
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_2",
                Url = "https://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "11389"
            });
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_3",
                Url = "https://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "1238912"
            });
            return repositoryConfigs;
        }
    }
}
