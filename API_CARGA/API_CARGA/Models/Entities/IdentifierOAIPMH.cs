// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Sirve encapsular los datos provenientes del ListIdentifiers
using System;
using System.Diagnostics.CodeAnalysis;

namespace API_CARGA.Models.Entities
{
    ///<summary>
    ///Sirve encapsular los datos provenientes del ListIdentifiers
    ///</summary>
    ///
    [ExcludeFromCodeCoverage]
    public class IdentifierOAIPMH
    {
        public string Identifier { get; set; }
        public DateTime Fecha { get; set; }
    }
}
