// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para guardar información sobre el estado en el que se encuntra la sincronización lanzada por una tarea
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Clase para guardar información sobre el estado en el que se encuntra la sincronización lanzada por una tarea
    /// </summary>
    public class ProcessingJobState
    {
        /// <summary>
        /// Identificador
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Repositorio sincronizado
        /// </summary>
        public Guid RepositoryId { get; set; }
        /// <summary>
        /// Tarea que ha lanzado el proceso
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// Número de elementos procesados
        /// </summary>
        public int ProcessNumIdentifierOAIPMH { get; set; }
        /// <summary>
        /// Número de elementos a procesar
        /// </summary>
        public int TotalNumIdentifierOAIPMH { get; set; }
        /// <summary>
        /// Último elemento procesado
        /// </summary>
        public string LastIdentifierOAIPMH { get; set; }
    }
}
