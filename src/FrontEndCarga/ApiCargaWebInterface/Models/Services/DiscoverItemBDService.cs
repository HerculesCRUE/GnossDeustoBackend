// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using ApiCargaWebInterface.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiCargaWebInterface.Models.Services
{
    ///<summary>
    ///Clase para gestionar las operaciones de las tareas de descubrimiento
    ///</summary>
    public class DiscoverItemBDService
    {
        private readonly EntityContext _context;
        public DiscoverItemBDService(EntityContext context)
        {
            _context = context;
        }


        ///<summary>
        /// Obtiene un item de descubrimiento
        ///</summary>
        ///<param name="id">Identificador del item</param>
        ///<remarks>Item de descubrimiento</remarks>
        public DiscoverItem GetDiscoverItemById(Guid id)
        {
            return _context.DiscoverItem.Include(item => item.DiscardDissambiguations).Include(item => item.DissambiguationProblems).ThenInclude(p => p.DissambiguationCandiates).FirstOrDefault(item => item.ID.Equals(id));
        }

        /// <summary>
        /// Obtiene los items con error de un Job (sólo obtiene el identificador y el estado)
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns>Lista de Items de descubrimiento (sólo obtiene el identificador y el estado)</returns>
        public List<DiscoverItem> GetDiscoverItemsErrorByJobMini(string jobId)
        {
            return _context.DiscoverItem.Where(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Error.ToString() || x.Status == DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString())).Select(x => new DiscoverItem { ID = x.ID, JobID = x.JobID, Status = x.Status }).ToList();
        }

        /// <summary>
        /// Obtiene el número de items en cada uno de los estados de descubrimiento
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public Dictionary<string, int> GetDiscoverItemsStatesByJob(string jobId)
        {
            return _context.DiscoverItem.Where(x => x.JobID == jobId).GroupBy(p => p.Status).Select(g => new { state = g.Key, count = g.Count() }).ToDictionary(k => k.state, i => i.count);
        }

        /// <summary>
        /// Obtiene si existen o no items pendientes de procesar por el descubrimiento para un Job
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsPending(string jobId)
        {
            return _context.DiscoverItem.Any(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Pending.ToString()));
        }

        /// <summary>
        /// Obtiene si existen o no items con estado error o procesados con problemas de desambiguación
        /// </summary>
        /// <param name="jobId">Identificador del job</param>
        /// <returns></returns>
        public bool ExistsDiscoverItemsErrorOrDissambiguatinProblems(string jobId)
        {
            return _context.DiscoverItem.Any(x => x.JobID == jobId && (x.Status == DiscoverItem.DiscoverItemStatus.Error.ToString() || x.Status == DiscoverItem.DiscoverItemStatus.ProcessedDissambiguationProblem.ToString()));
        }

        ///<summary>
        ///Añade un item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">Item de descubrimiento</param>
        public Guid AddDiscoverItem(DiscoverItem discoverItem)
        {
            discoverItem.ID = Guid.NewGuid();
            _context.DiscoverItem.Add(discoverItem);
            _context.SaveChanges();
            return discoverItem.ID;
        }

        ///<summary>
        ///Modifica una item de descubrimiento
        ///</summary>
        ///<param name="discoverItem">tem de descubrimiento a modificar con los datos nuevos</param>
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

                _context.SaveChanges();
                modified = true;
            }
            return modified;
        }

        ///<summary>
        ///Elimina un discoverItem
        ///</summary>
        ///<param name="identifier">Identificador del item</param>
        public bool RemoveDiscoverItem(Guid identifier)
        {
            try
            {
                DiscoverItem discoverItem = GetDiscoverItemById(identifier);
                if (discoverItem != null)
                {
                    if (discoverItem.DissambiguationProblems != null)
                    {
                        foreach (var dissambiguationProblem in discoverItem.DissambiguationProblems)
                        {
                            _context.Entry(dissambiguationProblem).State = EntityState.Deleted;

                            if (dissambiguationProblem.DissambiguationCandiates != null)
                            {
                                foreach (var dissambiguationCandidate in dissambiguationProblem.DissambiguationCandiates)
                                {
                                    _context.Entry(dissambiguationCandidate).State = EntityState.Deleted;
                                }
                            }
                        }
                    }

                    _context.Entry(discoverItem).State = EntityState.Deleted;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
