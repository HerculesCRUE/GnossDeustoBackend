// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// almacena los nombres
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ApiCargaWebInterface.Models.UrisFactory
{
    ///<summary>
    ///almacena los nombres 
    ///</summary>
    public static class UriComponentsList
    {
        /// <summary>
        /// Base
        /// </summary>
        public const string Base = "base";
        /// <summary>
        /// Character
        /// </summary>
        public const string Character = "character";
        /// <summary>
        /// ResourceClass
        /// </summary>
        public const string ResourceClass = "resourceClass";
        /// <summary>
        /// Identifier
        /// </summary>
        public const string Identifier = "identifier";

        private static ImmutableList<String> defaultParameters = new List<string> { Base, Character, ResourceClass, Identifier }.ToImmutableList();

        /// <summary>
        /// DefaultParameters
        /// </summary>
        public static ImmutableList<String> DefaultParameters
        {
            get
            {
                return defaultParameters;
            }
        }
    }
}
