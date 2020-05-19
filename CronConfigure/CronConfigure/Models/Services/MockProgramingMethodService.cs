using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    public class MockProgramingMethodService : IProgramingMethodService
    {
        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
            return "";
        }

        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null)
        {
           
        }

        public string PublishRepositories(Guid idRepositoryGuid, DateTime? fecha = null, string pSet = null, string codigoObjeto = null)
        {
            return "";
        }
    }
}
