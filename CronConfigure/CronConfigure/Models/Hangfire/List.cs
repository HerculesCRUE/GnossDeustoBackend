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
    [Table("list", Schema = "hangfire")]
    public partial class List
    {
        [Column(Order = 0)]
        public long Id { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}
