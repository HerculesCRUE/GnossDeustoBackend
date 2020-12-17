// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve para mostrar un ejemplo de respuesta de un repositorios
using API_CARGA.Models.Entities;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.ModelExamples
{
    ///<summary>
    ///Sirve para mostrar un ejemplo de respuesta de un repositorios
    ///</summary>
    ///
    [ExcludeFromCodeCoverage]
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
