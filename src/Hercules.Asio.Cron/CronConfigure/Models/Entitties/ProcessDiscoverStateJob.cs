// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para asociar una tarea con su estado de descubrimiento
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models.Entitties
{
    /// <summary>
    /// Clase para asociar una tarea con su estado de descubrimiento
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Table("ProcessDiscoverStateJob")]
    public class ProcessDiscoverStateJob
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// JobId
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
    }
}
