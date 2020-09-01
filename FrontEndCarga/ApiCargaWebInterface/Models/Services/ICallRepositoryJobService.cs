// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para hacer llamadas al api de Cron
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Interfaz para hacer llamadas al api de Cron
    /// </summary>
    public interface ICallRepositoryJobService
    {
        /// <summary>
        /// Obtiene las tareas recurrentes de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas recurrentes</returns>
        public List<RecurringJobViewModel> GetRecurringJobsOfRepo(Guid idRepositoy);
        /// <summary>
        /// Obtiene las tareas de sincronización ejecutadas de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobsOfRepo(Guid idRepositoy);
        /// <summary>
        /// Obtiene las tareas programadas de un repositorio OAIPMH
        /// </summary>
        /// <param name="idRepositoy">Identificador del repositorio</param>
        /// <returns>Lista de tareas programadas</returns>
        public List<ScheduledJobViewModel> GetScheduledJobsOfRepo(Guid idRepositoy);
    }
}
