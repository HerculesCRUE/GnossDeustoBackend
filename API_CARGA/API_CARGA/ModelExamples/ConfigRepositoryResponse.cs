using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.ModelExamples
{
    public class ConfigRepositoryResponse : IExamplesProvider<RepositoryConfig>
    {
        public RepositoryConfig GetExamples()
        {
            return new RepositoryConfig
            {
                RepositoryConfigID = Guid.NewGuid(),
                Name = "ConfigRepository_1",
                Url = "http://herc-as-front-desa.atica.um.es/oai-pmh-cvn/OAI_PMH",
                OauthToken = "12weq1"
            };
        }
    }
}
