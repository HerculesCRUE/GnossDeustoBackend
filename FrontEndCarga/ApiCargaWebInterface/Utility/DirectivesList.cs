using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Utility
{
    public class DirectivesList
    {
        private static string statisticsGrafo = "@*<% use statistics param:grafo /%>*@";
        private static string statisticsTriples = "@*<% use statistics param:triples /%>*@";
        private static string statisticsEntidades = "@*<% use statistics param:entidades /%>*@";
        private static string sparql = "@*<% use sparql /%>*@";
        private static string directivaJavi = "@*<% use Javi /%>*@";

        public static string StatisticsGrafo
        {
            get { return statisticsGrafo; }
        }

        public static string StatisticsTriples
        {
            get { return statisticsTriples; }
        }

        public static string StatisticsEntidades
        {
            get { return statisticsEntidades; }
        }

        public static string Sparql
        {
            get { return sparql; }
        }
        public static string DirectivaJavi
        {
            get { return directivaJavi; }
        }
    }
}
