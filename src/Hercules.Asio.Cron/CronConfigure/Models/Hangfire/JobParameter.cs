// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    ///[ExcludeFromCodeCoverage]
    [Table("jobparameter", Schema = "hangfire")]
    public partial class JobParameter
    {
        /// <summary>
        /// JobId
        /// </summary>
        [Column("jobid",Order = 0)]
        public long JobId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Column("name", Order = 1)]
        [StringLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [Column("value", Order = 2)]
        public string Value { get; set; }

        /// <summary>
        /// Job
        /// </summary>
        public virtual Job Job { get; set; }
    }
}
