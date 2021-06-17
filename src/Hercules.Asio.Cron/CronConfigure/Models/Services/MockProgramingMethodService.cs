// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve de mock del ProgramingMethodService para la realización de los test
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using System;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase que sirve de mock del ProgramingMethodService para la realización de los test
    ///</summary>
    public class MockProgramingMethodService : IProgramingMethodService
    {
        /// <summary>
        /// ProgramPublishRepositoryJob
        /// </summary>
        /// <param name="idRepository"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="set"></param>
        /// <param name="codigoObjeto"></param>
        /// <returns></returns>
        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, string set = null, string codigoObjeto = null)
        {
            return "";
        }

        /// <summary>
        /// ProgramPublishRepositoryRecurringJob
        /// </summary>
        /// <param name="idRepository"></param>
        /// <param name="nombreCron"></param>
        /// <param name="cronExpression"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="configuration"></param>
        /// <param name="set"></param>
        /// <param name="codigoObjeto"></param>
        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, IConfiguration configuration, string set = null, string codigoObjeto = null)
        {
           // Método sobreescrito.
        }

        /// <summary>
        /// PublishRepositories
        /// </summary>
        /// <param name="idRepositoryGuid"></param>
        /// <param name="context"></param>
        /// <param name="pSet"></param>
        /// <param name="codigoObjeto"></param>
        /// <returns></returns>
        public string PublishRepositories(Guid idRepositoryGuid, PerformContext context, string pSet = null, string codigoObjeto = null)
        {
            return "";
        }
    }
}
