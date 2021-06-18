using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private IConnection _connection;
        private IModel _channel;
        private string QueueName;
        private IConfiguration _configuration;
        private string _urlSparqlServer;
        private string _userSparqlServer;
        private string _passwordSparqlServer;
        readonly IDictionary _environmentVariables;
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
            _urlSparqlServer = GetConfiguration("SparqlServer_Url");
            ConnectionFactory _connectionFactory = new ConnectionFactory();

            _connectionFactory.HostName = GetConfiguration("RabbitMQ_Hostname");
            _connectionFactory.UserName = GetConfiguration("RabbitMQ_User");
            _connectionFactory.Password = GetConfiguration("RabbitMQ_Password");
            _connectionFactory.VirtualHost = GetConfiguration("RabbitMQ_VirtualHost");
            QueueName = GetConfiguration("RabbitMQ_QueueName");
            _userSparqlServer = GetConfiguration("SparqlServer_User");
            _passwordSparqlServer = GetConfiguration("SparqlServer_Password");
            _connectionFactory.Port = AmqpTcpEndpoint.UseDefaultPort;
            _connectionFactory.DispatchConsumersAsync = true;

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(QueueName, true, false, false);            
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

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
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

            try
            {

                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                if (!string.IsNullOrEmpty(_userSparqlServer) && !string.IsNullOrEmpty(_passwordSparqlServer))
                {
                    webClient.Credentials = new System.Net.NetworkCredential(_userSparqlServer, _passwordSparqlServer);
                }

                NameValueCollection parametros = new NameValueCollection();
                parametros.Add("default-graph-uri", queryVirtuoso.graph);
                parametros.Add("query", queryVirtuoso.query);
                parametros.Add("format", "application/sparql-results+json");

                byte[] responseArray = null;
                int numIntentos = 0;
                Exception exception = null;
                while (responseArray == null && numIntentos < 5)
                {
                    numIntentos++;
                    try
                    {
                        responseArray = webClient.UploadValues(_urlSparqlServer, "POST", parametros);
                        exception = null;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(10000);
                        exception = ex;
                    }
                }
                if (exception != null)
                {
                    throw exception;
                }
                webClient.Dispose();
                return "";
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, e.Message);
                throw;
            }
        }
    }
}
