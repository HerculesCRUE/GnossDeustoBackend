// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using API_DISCOVER.Models.Entities;

namespace API_DISCOVER.Utility
{
    public interface I_SparqlUtility
    {
        public SparqlObject SelectData(string pSPARQLEndpoint, string pGraph, string pConsulta, string pQueryParam);
    }
}
