// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para obtener la información de las tareas vinculadas a un repositorio
using CronConfigure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    /// <summary>
    /// Interfaz para obtener la información de las tareas vinculadas a un repositorio
    /// </summary>
    public interface IRepositoryCronService
    {
        ///<summary>
        ///Obtiene las tareas recurrentes de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas recurrentes</returns>
        public List<RecurringJobViewModel> GetRecurringJobs(Guid repositoryID);
        ///<summary>
        ///Obtiene las tareas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobs(Guid repositoryID);
        ///<summary>
        ///Obtiene las tareas programadas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas programadas</returns>
        public List<ScheduledJobViewModel> GetScheduledJobs(Guid repositoryID);
        ///<summary>
        ///Obtiene las tareas ejecutadas a partir de todas las tareas recurrentes vinculadas a un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetJobsOfRecurringJob(Guid repositoryID);
        ///<summary>
        ///Obtiene las tareas recurrentes y las tareas de un repositorio
        ///</summary>
        ///<param name="repositoryID">Identificador del repositorio</param>
        ///<returns>Lista de tareas</returns>
        public List<JobViewModel> GetAllJobs(Guid repositoryID);
    }
}
