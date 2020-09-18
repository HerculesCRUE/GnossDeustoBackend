﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using API_DISCOVER.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API_DISCOVER.Models.Services
{
    ///<summary>
    ///Clase para gestionar los estados de descubrimiento de las tareas
    ///</summary>
    public class ProcessDiscoverStateJobBDService
    {
        private readonly EntityContext _context;
        public ProcessDiscoverStateJobBDService(EntityContext context)
        {
            _context = context;
        }


        ///<summary>
        ///Obtiene un estado de descubrimiento de una tarea a través de su identificador
        ///</summary>
        ///<param name="id">Identificador del estado de descubrimiento de la tarea</param>
        public ProcessDiscoverStateJob GetrocessDiscoverStateJobById(Guid id)
        {
            return _context.ProcessDiscoverStateJob.FirstOrDefault(item => item.Id.Equals(id));
        }

        ///<summary>
        ///Obtiene un estado de descubrimiento de una tarea a través de su idJob
        ///</summary>
        ///<param name="id">Identificador del Job del estado de descubrimiento de la tarea</param>
        public ProcessDiscoverStateJob GetrocessDiscoverStateJobByIdJob(string idJob)
        {
            return _context.ProcessDiscoverStateJob.FirstOrDefault(item => item.JobId.Equals(idJob));
        }

        ///<summary>
        ///Añade un estado de descubrimiento de una tarea
        ///</summary>
        ///<param name="processDiscoverStateJob">Estado de descubrimiento de una tarea</param>
        public Guid AddProcessDiscoverStateJob(ProcessDiscoverStateJob processDiscoverStateJob)
        {
            if (processDiscoverStateJob.Id == Guid.Empty)
            {
                processDiscoverStateJob.Id = Guid.NewGuid();
            }
            _context.ProcessDiscoverStateJob.Add(processDiscoverStateJob);
            _context.SaveChanges();
            return processDiscoverStateJob.Id;
        }

        ///<summary>
        ///Modifica un estado de descubrimiento de una tarea
        ///</summary>
        ///<param name="processDiscoverStateJob">Estado de descubrimiento de una tarea</param>
        public bool ModifyProcessDiscoverStateJob(ProcessDiscoverStateJob processDiscoverStateJob)
        {
            bool modified = false;
            ProcessDiscoverStateJob processDiscoverStateJobOriginal = GetrocessDiscoverStateJobById(processDiscoverStateJob.Id);
            if (processDiscoverStateJobOriginal != null)
            {
                processDiscoverStateJobOriginal.JobId = processDiscoverStateJob.JobId;
                processDiscoverStateJobOriginal.State = processDiscoverStateJob.State;                            
                _context.SaveChanges();
                modified = true;
            }
            return modified;
        }

        ///<summary>
        ///Elimina un estado de descubrimiento de una tarea
        ///</summary>
        ///<param name="id">Identificador del estado de descubrimiento de la tarea</param>
        public bool RemoveDiscoverItem(Guid id)
        {
            try
            {
                ProcessDiscoverStateJob processDiscoverStateJob = GetrocessDiscoverStateJobById(id);
                if (processDiscoverStateJob != null)
                {
                    _context.Entry(processDiscoverStateJob).State = EntityState.Deleted;
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
