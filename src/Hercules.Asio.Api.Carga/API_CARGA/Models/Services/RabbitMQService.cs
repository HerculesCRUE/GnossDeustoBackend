// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para conectar con el servidor Rabbit
using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    /// <summary>
    /// Clase para conectar con el servidor Rabbi
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQInfo amqpInfo;
        private readonly ConnectionFactory connectionFactory;
        public string queueName { get; set; }
        public string queueNameVirtuoso { get; set; }

        private IConfiguration _configuration { get; set; }

        /// <summary>
        /// Constructor de la clase que configura los datos necesarios para conectarse con rabbit
        /// </summary>
        /// <param name="ampOptionsSnapshot">Opciones de configuracion para Rabbit</param>
        /// <param name="configuration">Configuración.</param>
        public RabbitMQService(IOptions<RabbitMQInfo> ampOptionsSnapshot, IConfiguration configuration)
        {
            _configuration = configuration;

            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            if (environmentVariables.Contains("RabbitQueueName"))
            {
                queueName = environmentVariables["RabbitQueueName"] as string;
            }
            else
            {
                queueName = _configuration["RabbitQueueName"];
            }

            if (environmentVariables.Contains("RabbitQueueNameVirtuoso"))
            {
                queueNameVirtuoso = environmentVariables["RabbitQueueNameVirtuoso"] as string;
            }
            else
            {
                queueNameVirtuoso = _configuration["RabbitQueueNameVirtuoso"];
            }

            amqpInfo = ampOptionsSnapshot.Value;

            connectionFactory = new ConnectionFactory
            {
                UserName = amqpInfo.UsernameRabbitMq,
                Password = amqpInfo.PasswordRabbitMq,
                VirtualHost = amqpInfo.VirtualHostRabbitMq,
                HostName = amqpInfo.HostNameRabbitMq,
                Uri = new Uri(amqpInfo.UriRabbitMq)
            };
        }

        /// <summary>
        /// Encola un objeto en Rabbit
        /// </summary>
        /// <param name="message">Objeto a encolar</param>
        /// <param name="queue">Cola</param>
        /// <param name="pDurable">Sí es durable o no</param>
        public void PublishMessage(object message, string queue, bool pDurable=false)
        {
            using (var conn = connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: queue,
                        durable: pDurable,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var jsonPayload = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonPayload);

                    channel.BasicPublish(exchange: "",
                        routingKey: queue,
                        basicProperties: null,
                        body: body
                    );
                }
            }
        }
    }
}
