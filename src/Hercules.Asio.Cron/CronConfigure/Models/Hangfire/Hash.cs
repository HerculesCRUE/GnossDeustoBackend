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
    [Table("hash", Schema = "hangfire")]
    public partial class Hash
    {
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string Field { get; set; }

        public string Value { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime? ExpireAt { get; set; }
    }
}
