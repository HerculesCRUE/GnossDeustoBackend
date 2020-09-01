// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para la programación de tareas
using Hangfire.Server;
using System;

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
        public string PublishRepositories(Guid idRepositoryGuid, PerformContext context, DateTime? fecha = null, string pSet = null, string codigoObjeto = null);
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
