// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para obtener información sobre el estado en el que se encuntra la sincronización lanzada por una tarea
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Entitties
{    
    ///<summary>
    ///Clase para obtener información sobre el estado en el que se encuntra la sincronización lanzada por una tarea
    ///</summary>
    [Table("ProcessingJobState")]
    [ExcludeFromCodeCoverage]
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
