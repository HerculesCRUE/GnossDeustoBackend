// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallService
    {
        public string CallGetApi(string urlMethod);
        public string CallPostApi(string urlMethod, object item, bool isFile = false);
        public string CallPutApi(string urlMethod, object item, bool isFile = false);
        public string CallDeleteApi(string urlMethod);
    }
}
