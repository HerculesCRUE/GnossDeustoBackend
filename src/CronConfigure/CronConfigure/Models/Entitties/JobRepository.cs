// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que vincula una sincronización con un repositorio
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.Models.Entitties
{
    [ExcludeFromCodeCoverage]
    [Table("JobRepository", Schema = "hangfire")]
    ///<summary>
    ///Clase que vincula una sincronización con un repositorio
    ///</summary>
    public class JobRepository
    {
        [Key]
        public string IdJob { get; set; }
        [Required]
        public Guid IdRepository { get; set; }
        public DateTime FechaEjecucion { get; set; }
    }
}
