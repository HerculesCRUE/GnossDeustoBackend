﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase de ejemplo para mostrar el correcto reemplazo de un fichero de configuración
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar el correcto reemplazo de un fichero de configuración
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class ReplaceSchemaResponse : IExamplesProvider<string>
    {
        /// <summary>
        /// GetExamples
        /// </summary>
        /// <returns></returns>
        public string GetExamples()
        {
            return "new config file loaded";
        }

    }
}
