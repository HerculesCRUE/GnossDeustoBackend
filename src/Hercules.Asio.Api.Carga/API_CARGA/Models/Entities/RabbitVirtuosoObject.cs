using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace API_CARGA.Models.Entities
{
    [ExcludeFromCodeCoverage]
    public class RabbitVirtuosoObject
    {
        public string graph { get; set; }
        public string query { get; set; }

        public RabbitVirtuosoObject(string pGraph, string pQuery)
        {
            this.graph = pGraph;
            this.query = pQuery;
        }
    }
}
