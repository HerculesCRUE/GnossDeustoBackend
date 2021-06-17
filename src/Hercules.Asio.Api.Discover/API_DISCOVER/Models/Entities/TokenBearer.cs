﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que representa un token de acceso
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Clase que representa un token de acceso
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TokenBearer
    {
        /// <summary>
        /// access_token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// expires_in
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// token_type
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// scope
        /// </summary>
        public string scope { get; set; }
    }
}
