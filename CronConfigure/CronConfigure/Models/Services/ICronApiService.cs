// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para la gestión los distintos tipos de tareas
using CronConfigure.Models.Enumeracion;
using CronConfigure.ViewModels;
using System.Collections.Generic;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Interfaz para la gestión los distintos tipos de tareas
    ///</summary>
    public interface ICronApiService
    {
        ///<summary>
        ///Obtiene una lista de tareas recurrentes
        ///</summary>
        public List<RecurringJobViewModel> GetRecurringJobs();
        ///<summary>
        ///Comprueba que exista una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public bool ExistJob(string id);
        ///<summary>
        ///Elimina una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public void DeleteJob(string id);
        ///<summary>
        ///Elimina una tarea recurrente
        ///</summary>
        ///<param name="id">nombre de la tarea</param>
        public void DeleteRecurringJob(string id);
        ///<summary>
        ///Pone en la cola una tarea una tarea
        ///</summary>
        ///<param name="id">identificador de la tarea</param>
        public void EnqueueJob(string id);
        ///<summary>
        ///Obtiene una tarea recurrente
        ///</summary>
        ///<param name="id">nombre de la tarea recurrente</param>
        public RecurringJobViewModel GetRecurringJobs(string id);
        ///<summary>
        ///Obtiene una lista de tareas ejecutadas
        ///</summary>
        ///<param name="type">tipo de la tarea</param>
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        public List<JobViewModel> GetJobs(JobType type, int from, int count);
        ///<summary>
        ///Obtiene una lista de tareas programadas
        ///</summary>
        /// <param name="from">número desde el cual se va a traer las tareas del listado, por defecto 0 para empezar a traer desde el primer elemento de la lista de tareas</param>
        /// <param name="count">número máximo de tareas a traer</param>
        public List<ScheduledJobViewModel> GetScheduledJobs(int from, int count);
        ///<summary>
        ///Obtiene una lista de tareas ejecutadas a partir de una tarea recurrente
        ///</summary>
        ///<param name="recurringJob">nombre de la tarea recurrente</param>
        public List<JobViewModel> GetJobsOfRecurringJob(string recurringJob);
        ///<summary>
        ///Comprueba si existe una tarea recurrente
        ///</summary>
        ///<param name="recurringJob">nombre de la tarea recurrente</param>
        public bool ExistRecurringJob(string recurringJob);
        ///<summary>
        ///Comprueba si existe una tarea programada
        ///</summary>
        ///<param name="id">identificador de la tarea programada</param>
        public bool ExistScheduledJob(string id);
        public JobViewModel GetJob(string id);
    }
}
