using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// Encola un objeto en Rabbbit
        /// </summary>
        /// <param name="message">Objeto a encolar</param>
        public void PublishMessage(object message);
    }
}
