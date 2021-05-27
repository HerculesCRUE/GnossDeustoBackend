using System;
using System.Collections.Generic;
using System.Text;

namespace Hercules.Asio.Api.Discover.Models.Entities
{
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
