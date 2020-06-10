using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un ejemplo de respuesta de una lista de repositorios 
    ///</summary>
    public class ConfigRepositoriesResponse : IExamplesProvider<List<RepositoryConfig>>
    {
        public List<RepositoryConfig> GetExamples()
        {
            List<RepositoryConfig> repositoryConfigs = new List<RepositoryConfig>();
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_1",
                Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "12weq1"
            });
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_2",
                Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "11389"
            });
            repositoryConfigs.Add(new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_3",
                Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "1238912"
            });
            return repositoryConfigs;
        }
    }
}
