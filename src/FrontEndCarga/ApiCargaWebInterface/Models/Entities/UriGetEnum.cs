// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Enumeración con las posibles elecciones en el resource class a la hora de obtener una uri
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public enum UriGetEnum
    {
        [Display(Name = "Resource class")]
        Resource_class = 0,
        [Display(Name = "RDFType")]
        RDFtype = 1
    }
}
