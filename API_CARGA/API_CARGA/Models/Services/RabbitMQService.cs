using API_CARGA.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class RabbitMQService
    {

        private readonly RabbitMQInfo amqpInfo;
        private readonly ConnectionFactory connectionFactory;
        private string queueName;

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

        public void PublishMessage(object message)
        {
            using (var conn = connectionFactory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var jsonPayload = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonPayload);

                    channel.BasicPublish(exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: body
                    );
                }
            }
        }
    }
}
