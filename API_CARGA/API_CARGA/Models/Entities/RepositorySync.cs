using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
