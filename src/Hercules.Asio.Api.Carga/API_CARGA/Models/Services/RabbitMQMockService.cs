using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    /// <summary>
    /// Interfaz para conectar con el servidor Rabbi
    /// </summary>
    public class RabbitMQMockService : IRabbitMQService
    {
        public string queueName { get; set; }
        public string queueNameVirtuoso { get; set; }
        public void PublishMessage(object message,string queue,bool durable)
        {
            
        }
    }
}
