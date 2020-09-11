// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_CARGA.Models.Services
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
        ///Obtiene una item de descubrimiento
        ///</summary>
        ///<param name="id">Identificador del item</param>
        public DiscoverItem GetDiscoverItemById(Guid id)
        {
            return _context.DiscoverItem.Include(item => item.DissambiguationProblems).ThenInclude(p => p.DissambiguationCandiates).FirstOrDefault(item => item.ID.Equals(id));
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
                discoverItemOriginal.JobCreatedDate = discoverItem.JobCreatedDate;
                discoverItemOriginal.Publish = discoverItem.Publish;
                discoverItemOriginal.DissambiguationProcessed = discoverItem.DissambiguationProcessed;
                discoverItemOriginal.DiscoverReport = discoverItem.DiscoverReport;
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
