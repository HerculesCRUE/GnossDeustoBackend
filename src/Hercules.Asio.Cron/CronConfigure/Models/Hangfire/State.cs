// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Hangfire
{
    [ExcludeFromCodeCoverage]
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [Table("state", Schema = "hangfire")]
    public partial class State
    {
        [Column(Order = 0)]
        public long Id { get; set; }

        [Column(Order = 1)]
        public long JobId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Data { get; set; }

        public virtual Job Job { get; set; }
    }
}
