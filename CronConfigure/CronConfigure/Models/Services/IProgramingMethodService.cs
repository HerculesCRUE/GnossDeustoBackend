using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Services
{
    ///<summary>
    ///Interfaz para la programación de tareas
    ///</summary>
    public interface IProgramingMethodService
    {
        ///<summary>
        ///Método para la sincronización de repositorios
        ///</summary>
        ///<param name="idRepositoryGuid">identificador del repositorio a sincronizar</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        /// <param name="set">tipo del objeto, usado para filtrar por agrupaciones
        /// <param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public string PublishRepositories(Guid idRepositoryGuid, DateTime? fecha = null, string pSet = null, string codigoObjeto = null);
        ///<summary>
        ///Método para programar una sincronización recurrente
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="nombreCron">Nombre de la tarea recurrente</param>
        ///<param name="cronExpression">expresión de recurrencia</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public static void ProgramRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime? fecha = null, string set = null, string codigoObjeto = null) { }
        ///<summary>
        ///Método para programar una tarea
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="fechaInicio">Fecha de la ejecución</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public string ProgramPublishRepositoryJob(Guid idRepository, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null);
        ///<summary>
        ///Método para programar una tarea con una sincronización recurrente
        ///</summary>
        ///<param name="idRepository">identificador del repositorio a sincronizar</param>
        ///<param name="nombreCron">Nombre de la tarea recurrente</param>
        ///<param name="cronExpression">expresión de recurrencia</param>
        ///<param name="fechaInicio">Fecha en la que se ejecutará la tarea y se activará la tarea recurrente</param>
        ///<param name="fecha">Fecha desde la que se quiere sincronizar</param>
        ///<param name="set">tipo del objeto, usado para filtrar por agrupaciones
        ///<param name="codigo_objeto">codigo del objeto a sincronizar, es necesario pasar el parametro set si se quiere pasar este parámetro</param>
        public void ProgramPublishRepositoryRecurringJob(Guid idRepository, string nombreCron, string cronExpression, DateTime fechaInicio, DateTime? fecha = null, string set = null, string codigoObjeto = null);


    }
}
