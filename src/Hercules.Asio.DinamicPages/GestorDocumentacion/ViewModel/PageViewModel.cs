// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;

namespace GestorDocumentacion.ViewModel
{
    public class PageViewModel
    {
        public Guid PageID { get; set; }
        public string Route { get; set; }
        public DateTime LastModified { get; set; }
    }
}
