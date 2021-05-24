using System;
using System.Collections.Generic;
using System.Text;

namespace Hercules.Asio.Api.Discover.Models.Entities
{
    class RabbitVirtuosoObject
    {
        private string graph { get; set; }
        private string query { get; set; }

        public RabbitVirtuosoObject(string pGraph, string pQuery)
        {
            this.graph = pGraph;
            this.query = pQuery;
        }
    }
}
