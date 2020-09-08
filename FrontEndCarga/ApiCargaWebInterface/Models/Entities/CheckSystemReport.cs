// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para guardar la información sobre el estado de los apis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public class CheckSystemReport
    {
        public bool ApiCarga { get; set; }
        public bool ApiCron { get; set; }
        public bool IdentityServer { get; set; }
    }
}
