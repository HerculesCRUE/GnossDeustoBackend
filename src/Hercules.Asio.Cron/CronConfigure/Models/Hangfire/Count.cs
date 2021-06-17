﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
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
    [Table("counter", Schema = "hangfire")]
    [ExcludeFromCodeCoverage]
    public class Count
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int Value { get; set; }
        /// <summary>
        /// ExpireAt
        /// </summary>
        public DateTime? ExpireAt { get; set; }
    }
}
