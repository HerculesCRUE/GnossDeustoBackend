// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase con el listado de directivas

namespace ApiCargaWebInterface.Utility
{
    /// <summary>
    /// Clase con el listado de directivas
    /// </summary>
    public class DirectivesList
    {
        private static string sparql = "@*<% sparql";
        private static string api = "@*<% api";
        private static string directive = "@*<%";
        private static string endDirective = "/%>*@";
        /// <summary>
        /// Obtiene el inicio de la directiva api
        /// </summary>
        public static string Api
        {
            get { return api; }
        }
        /// <summary>
        /// Obtiene el inicio de la directiva sparql
        /// </summary>
        public static string Sparql
        {
            get { return sparql; }
        }
        /// <summary>
        /// Obtiene la etiqueta de inicio de una directiva
        /// </summary>
        public static string Directive
        {
            get { return directive; }
        }
        /// <summary>
        /// Obtiene la etiqueta de fin de una directiva
        /// </summary>
        public static string EndDirective
        {
            get { return endDirective; }
        }
    }
}
