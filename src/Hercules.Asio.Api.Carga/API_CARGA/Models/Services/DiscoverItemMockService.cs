// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace API_CARGA.Models.Services
{
    ///<summary>
    ///Clase para gestionar las operaciones de las tareas de descubrimiento
    ///</summary>
    public class DiscoverItemMockService : IDiscoverItemService
    {
        readonly private List<DiscoverItem> _discoverItems;

        public DiscoverItemMockService()
        {
            _discoverItems = new List<DiscoverItem>();
            
            _discoverItems.Add(new DiscoverItem
            {
                ID = Guid.NewGuid(),
                Status = "Pending"
            });
        }


        ///<summary>
        /// Obtiene un item de descubrimiento
        ///</summary>
        ///<param name="id">Identificador del item</param>
        ///<remarks>Item de descubrimiento</remarks>
        public DiscoverItem GetDiscoverItemById(Guid id)
        {
            return _discoverItems.FirstOrDefault(discoverItem => discoverItem.ID.Equals(id));
        }

        /// <summary>
        /// Obtiene los items con error de un Job (sólo obtiene el identificador y el estado)
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns>Lista de Items de descubrimiento (sólo obtiene el identificador y el estado)</returns>
        public List<DiscoverItem> GetDiscoverItemsErrorByJobMini(string jobId)
        {
            return _discoverItems.Where(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Error.ToString() || x.Status == DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString())).Select(x => new DiscoverItem { ID = x.ID, JobID = x.JobID, Status = x.Status }).ToList();
        }

        /// <summary>
        /// Obtiene el número de items en cada uno de los estados de descubrimiento
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public Dictionary<string, int> GetDiscoverItemsStatesByJob(string jobId)
        {
            return _discoverItems.Where(x => x.JobID == jobId).GroupBy(p => p.Status).Select(g => new { state = g.Key, count = g.Count() }).ToDictionary(k => k.state, i => i.count);
        }

        /// <summary>
        /// Obtiene si existen o no items pendientes de procesar por el descubrimiento para un Job
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsPending(string jobId)
        {
            return _discoverItems.Any(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Pending.ToString()));
        }

        /// <summary>
        /// Obtiene si existen o no items con estado error o procesados con problemas de desambiguación
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsErrorOrDissambiguatinProblems(string jobId)
        {
            return _discoverItems.Any(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Error.ToString() || x.Status == DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString()));
        }

        ///<summary>
        ///Añade un item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">Item de descubrimiento</param>
        public Guid AddDiscoverItem(DiscoverItem discoverItem)
        {
            Guid discoveritemID = Guid.NewGuid();
            discoverItem.ID = discoveritemID;
            _discoverItems.Add(discoverItem);
            return discoveritemID;
        }

        ///<summary>
        ///Modifica una item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">tem de descubrimiento a modificar con los datos nuevos</param>
        [ExcludeFromCodeCoverage]
        public bool ModifyDiscoverItem(DiscoverItem discoverItem)
        {
            bool modified = false;
            DiscoverItem discoverItemOriginal = GetDiscoverItemById(discoverItem.ID);
            if (discoverItemOriginal != null)
            {
                discoverItemOriginal.Status = discoverItem.Status;
                discoverItemOriginal.Rdf = discoverItem.Rdf;
                discoverItemOriginal.DiscoverRdf = discoverItem.DiscoverRdf;
                discoverItemOriginal.Error = discoverItem.Error;
                discoverItemOriginal.JobID = discoverItem.JobID;
                discoverItemOriginal.Publish = discoverItem.Publish;
                discoverItemOriginal.DissambiguationProcessed = discoverItem.DissambiguationProcessed;
                discoverItemOriginal.DiscoverReport = discoverItem.DiscoverReport;
                discoverItemOriginal.DissambiguationProblems = discoverItem.DissambiguationProblems;
                discoverItemOriginal.DiscardDissambiguations = discoverItem.DiscardDissambiguations;
                discoverItemOriginal.LoadedEntities = discoverItem.LoadedEntities;

                modified = true;
            }
            return modified;
        }

        ///<summary>
        ///Elimina un discoverItem
        ///</summary>
        ///<param name="identifier">Identificador del item</param>
        [ExcludeFromCodeCoverage]
        public bool RemoveDiscoverItem(Guid identifier)
        {
            try
            {
                DiscoverItem discoverItem = GetDiscoverItemById(identifier);
                if (discoverItem != null)
                {
                    _discoverItems.Remove(discoverItem);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
