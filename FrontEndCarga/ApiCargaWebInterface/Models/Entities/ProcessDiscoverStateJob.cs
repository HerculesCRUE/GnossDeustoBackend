// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para asociar una tarea con su estado de descubrimiento
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Clase para asociar una tarea con su estado de descubrimiento
/// </summary>
namespace ApiCargaWebInterface.Models.Entities
{
    [Table("ProcessDiscoverStateJob")]
    public class ProcessDiscoverStateJob
    {
        [Key]
        public Guid Id { get; set; }
        public string JobId { get; set; }
        public string State { get; set; }
    }
}
