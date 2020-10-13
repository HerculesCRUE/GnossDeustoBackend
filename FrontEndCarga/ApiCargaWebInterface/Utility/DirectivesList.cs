using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Utility
{
    public class DirectivesList
    {
        private static string sparql = "@*<% sparql";
        private static string api = "@*<% api";
        private static string directive = "@*<%";
        private static string endDirective = "/%>*@";

        public static string Api
        {
            get { return api; }
        }

        public static string Sparql
        {
            get { return sparql; }
        }

        public static string Directive
        {
            get { return directive; }
        }
        public static string EndDirective
        {
            get { return endDirective; }
        }
    }
}
