using System;
using System.Collections.Generic;
using System.Text;

namespace Hercules.Asio.Api.Discover.Models.Entities
{
    /// <summary>
    /// RabbitVirtuosoObject
    /// </summary>
    public class RabbitVirtuosoObject
    {
        /// <summary>
        /// graph
        /// </summary>
        public string graph { get; set; }
        /// <summary>
        /// query
        /// </summary>
        public string query { get; set; }
        /// <summary>
        /// RabbitVirtuosoObject
        /// </summary>
        /// <param name="pGraph"></param>
        /// <param name="pQuery"></param>
        public RabbitVirtuosoObject(string pGraph, string pQuery)
        {
            this.graph = pGraph;
            this.query = pQuery;
        }
    }
}
