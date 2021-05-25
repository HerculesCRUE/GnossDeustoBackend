using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hercules.Asio.SPARQLReplication.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Hercules.Asio.SPARQLReplication
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string QueueName;
        private IConfiguration _configuration;
        private string _urlSparqlServer;
        private string _userSparqlServer;
        private string _passwordSparqlServer;
        IDictionary _environmentVariables;
        public Worker(IServiceProvider services, ILogger<Worker> logger)
        {
            Services = services;
            _logger = logger;
            _environmentVariables = Environment.GetEnvironmentVariables();
        }
        public IServiceProvider Services { get; }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            _urlSparqlServer = GetConfiguration("SparqlServer:Url");
            _connectionFactory = new ConnectionFactory();

            _connectionFactory.HostName = GetConfiguration("RabbitMQ:Hostname");
            _connectionFactory.UserName = GetConfiguration("RabbitMQ:User");
            _connectionFactory.Password = GetConfiguration("RabbitMQ:Password");
            _connectionFactory.VirtualHost = GetConfiguration("RabbitMQ:VirtualHost");
            QueueName = GetConfiguration("RabbitMQ:QueueName");
            _userSparqlServer = GetConfiguration("SparqlServer:User");
            _passwordSparqlServer = GetConfiguration("SparqlServer:Password");
            _connectionFactory.Port = AmqpTcpEndpoint.UseDefaultPort;
            _connectionFactory.DispatchConsumersAsync = true;

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(QueueName, true, false, false);
            //_channel.QueueDeclarePassive(QueueName);
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        private string GetConfiguration(string key)
        {
            string value = null;
            if (_environmentVariables.Contains(key))
            {
                value = _environmentVariables[key] as string;
            }
            else
            {
                value = _configuration[key];
            }

            return value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {       
            stoppingToken.ThrowIfCancellationRequested();
            string resultadoWebRequest = "";
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (bc, ea) =>
            {
                var messageQuery = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"Processing msg: '{messageQuery}'.");
                try
                {
                    resultadoWebRequest = await SendWebRequest(messageQuery);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception e)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    _logger.LogError(default, e, e.Message);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }
        private async Task<string> SendWebRequest(string query)
        {
            //deserialize
            QueryVirtuoso queryVirtuoso = System.Text.Json.JsonSerializer.Deserialize<QueryVirtuoso>(query);
            string respuesta = "";
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
                    var requestContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("query", queryVirtuoso.query),
                        new KeyValuePair<string, string>("format", "text/html"),
                        new KeyValuePair<string, string>("timeout", "0"),
                        new KeyValuePair<string, string>("default-graph-uri", queryVirtuoso.graph)
                    });

                    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{_userSparqlServer}:{_passwordSparqlServer}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);


                    response = await client.PostAsync(_urlSparqlServer, requestContent);
                    respuesta = await response.Content.ReadAsStringAsync();

                    return respuesta;
                }
                catch (Exception e)
                {
                    _logger.LogError(default, e, e.Message);
                    if (response == null)
                    {
                        response = new HttpResponseMessage();
                    }
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.ReasonPhrase = string.Format("Request failed");
                    throw new Exception(response.ReasonPhrase + " - " + response.StatusCode);
                }
            }
        }

    }
}
