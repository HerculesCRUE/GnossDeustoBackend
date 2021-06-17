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
    ///<summary>
    ///Clase usada por Hangfire para el guardado de las tareas
    ///</summary>
    [ExcludeFromCodeCoverage]
    [Table("server", Schema = "hangfire")]
    public partial class Server
    {
        /// <summary>
        /// Id
        /// </summary>
        [StringLength(100)]
        public string Id { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// LastHeartbeat
        /// </summary>
        public DateTime LastHeartbeat { get; set; }
    }
}
