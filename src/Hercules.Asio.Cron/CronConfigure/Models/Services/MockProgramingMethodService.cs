// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve de mock del ProgramingMethodService para la realización de los test
using Hangfire.Server;
using System;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Clase que sirve de mock del ProgramingMethodService para la realización de los test
    ///</summary>
    public class MockProgramingMethodService : IProgramingMethodService
    {
        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, string set = null, string codigoObjeto = null)
        {
            return "";
        }

        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, string set = null, string codigoObjeto = null)
        {
           
        }

        public string PublishRepositories(Guid idRepositoryGuid, PerformContext context, string pSet = null, string codigoObjeto = null)
        {
            return "";
        }
    }
}
