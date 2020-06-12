// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para pasar datos entre apis
using System;
using System.ComponentModel.DataAnnotations;

namespace API_CARGA.ViewModel
{
    /// <summary>
    /// Clase para pasar datos entre apis
    /// </summary>
    public class PublishRepositoryModel
    {
        [Required]
        public Guid repository_identifier { get; set; }
        public DateTime? fecha_from { get; set; }
        public string set { get; set; }
        //public string codigo_objeto { get; set; }

    }
}
