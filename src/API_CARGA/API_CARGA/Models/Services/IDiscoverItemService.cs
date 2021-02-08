using API_CARGA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Interfaz para gestionar las operaciones de las tareas de descubrimiento
    ///</summary>
    public interface IDiscoverItemService
    {
        ///<summary>
        /// Obtiene un item de descubrimiento
        ///</summary>
        ///<param name="id">Identificador del item</param>
        ///<remarks>Item de descubrimiento</remarks>
        public DiscoverItem GetDiscoverItemById(Guid id);

        /// <summary>
        /// Obtiene los items con error de un Job (sólo obtiene el identificador y el estado)
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns>Lista de Items de descubrimiento (sólo obtiene el identificador y el estado)</returns>
        public List<DiscoverItem> GetDiscoverItemsErrorByJobMini(string jobId);

        /// <summary>
        /// Obtiene el número de items en cada uno de los estados de descubrimiento
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public Dictionary<string, int> GetDiscoverItemsStatesByJob(string jobId);

        /// <summary>
        /// Obtiene si existen o no items pendientes de procesar por el descubrimiento para un Job
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsPending(string jobId);

        /// <summary>
        /// Obtiene si existen o no items con estado error o procesados con problemas de desambiguación
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsErrorOrDissambiguatinProblems(string jobId);

        ///<summary>
        ///Añade un item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">Item de descubrimiento</param>
        public Guid AddDiscoverItem(DiscoverItem discoverItem);

        ///<summary>
        ///Modifica una item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">tem de descubrimiento a modificar con los datos nuevos</param>
        public bool ModifyDiscoverItem(DiscoverItem discoverItem);

        ///<summary>
        ///Elimina un discoverItem
        ///</summary>
        ///<param name="identifier">Identificador del item</param>
        public bool RemoveDiscoverItem(Guid identifier);
    }
    
}
