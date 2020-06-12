// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase usada por Hangfire para el guardado de las tareas
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CronConfigure.Models.Hangfire
{
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [Table("set", Schema = "hangfire")]
    public partial class Set
    {
        [Column("key", Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        [Column("score")]
        public double Score { get; set; }

        [Column("value",Order = 1)]
        [StringLength(256)]
        public string Value { get; set; }

        [Column("expireat")]
        public DateTime? ExpireAt { get; set; }
    }
}
