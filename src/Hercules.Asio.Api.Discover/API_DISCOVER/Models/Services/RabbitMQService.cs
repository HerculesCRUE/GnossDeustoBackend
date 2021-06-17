// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para conectar con el servidor Rabbit

using API_DISCOVER.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER.Models.Services
{
    /// <summary>
    /// Clase para conectar con el servidor Rabbi
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RabbitMQService 
    {
        /// <summary>
        /// ReceivedDelegate
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public delegate bool ReceivedDelegate(string s);
        /// <summary>
        /// ShutDownDelegate
        /// </summary>
        public delegate void ShutDownDelegate();

        private IConnection connection = null;

        private readonly RabbitMQInfo amqpInfo;
        private readonly ConnectionFactory connectionFactory;
        private string queueName;
        /// <summary>
        /// queueNameVirtuoso
        /// </summary>
        public string queueNameVirtuoso { get; set; }

        /// <summary>
        /// Constructor de la clase que configura los datos necesarios para conectarse con rabbit
        /// </summary>
        /// <param name="ampOptionsSnapshot">Opciones de configuracion para Rabbit</param>
        public RabbitMQService(IOptions<RabbitMQInfo> ampOptionsSnapshot)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            IConfigurationRoot Configuration = builder.Build();
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            if (environmentVariables.Contains("RabbitQueueName"))
            {
                queueName = environmentVariables["RabbitQueueName"] as string;
            }
            else
            {
                queueName = Configuration["RabbitQueueName"];
            }

            if (environmentVariables.Contains("RabbitQueueNameVirtuoso"))
            {
                queueNameVirtuoso = environmentVariables["RabbitQueueNameVirtuoso"] as string;
            }
            else
            {
                queueNameVirtuoso = Configuration["RabbitQueueNameVirtuoso"];
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

            connection = connectionFactory.CreateConnection();
        }

        /// <summary>
        /// ListenToQueue
        /// </summary>
        /// <param name="receivedFunction"></param>
        /// <param name="shutdownFunction"></param>
        public void ListenToQueue(ReceivedDelegate receivedFunction, ShutDownDelegate shutdownFunction)
        {
            IModel channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);

            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                try
                {
                    IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties;
                    string body = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body.ToArray());

                    if (receivedFunction(body))
                    {
                        channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                    }
                }
                catch (Exception)
                {
                    channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                    throw;
                }
            };

            eventingBasicConsumer.Shutdown += (sender, shutdownEventArgs) =>
            {
                shutdownFunction();
            };

            channel.BasicConsume(queueName, false, eventingBasicConsumer);
        }

        /// <summary>
        /// Encola un objeto en Rabbbit
        /// </summary>
        /// <param name="message">Objeto a encolar</param>
        /// <param name="pQueue">Cola</param>
        public void PublishMessage(object message, string pQueue)
        {
            using (var conn = connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: pQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var jsonPayload = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonPayload);

                    channel.BasicPublish(exchange: "",
                        routingKey: pQueue,
                        basicProperties: null,
                        body: body
                    );
                }
            }
        }
    }
}
