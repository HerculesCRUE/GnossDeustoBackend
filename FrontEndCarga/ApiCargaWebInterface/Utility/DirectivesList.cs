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


        public static string Api
        {
            get { return api; }
        }

        public static string Sparql
        {
            get { return sparql; }
        }
    }
}
