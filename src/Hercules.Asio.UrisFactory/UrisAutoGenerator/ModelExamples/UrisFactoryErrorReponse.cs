﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase de ejemplo para mostrar un error al obtener una uri
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace UrisFactory.ModelExamples
{
    ///<summary>
    ///Clase de ejemplo para mostrar un error al obtener una uri
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class UrisFactoryErrorReponse : IExamplesProvider<UriErrorExample>
    {
        /// <summary>
        /// GetExamples
        /// </summary>
        /// <returns></returns>
        public UriErrorExample GetExamples()
        {
            return new UriErrorExample()
            {
                Error = "resource class: 'resercher' not configured"
            };
        }
    }
}
