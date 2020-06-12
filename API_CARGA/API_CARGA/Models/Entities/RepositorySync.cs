// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Datos de una sincornización sobre un repositorio
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_CARGA.Models.Entities
{
    /// <summary>
    /// Datos de una sincornización sobre un repositorio
    /// </summary>
    [Table("Sincronizacion_Repositorio")]
    public class RepositorySync
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RepositoryId { get; set; }
        public string Set { get; set; }
        public DateTime UltimaFechaDeSincronizacion{ get; set; }
    }
}
