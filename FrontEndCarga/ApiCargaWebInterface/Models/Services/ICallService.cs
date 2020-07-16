// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
﻿using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public interface ICallService
    {
        public string CallGetApi(string urlBase, string urlMethod, TokenBearer token = null);
        public string CallPostApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
        public string CallPostApiFiles(string urlBase, string urlMethod, Dictionary<string, IFormFile> files, TokenBearer token = null);
        public string CallPutApi(string urlBase, string urlMethod, object item, TokenBearer token = null, bool isFile = false, string fileName = "rdfFile");
        public string CallDeleteApi(string urlBase, string urlMethod, TokenBearer token = null);
    }
}
