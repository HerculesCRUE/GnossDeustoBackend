using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Logging;
using API_DISCOVER.Models.Services;
using API_DISCOVER.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Quartz;
using API_DISCOVER.Models.Logging;
using System.Collections.Generic;
using API_DISCOVER.Models.Entities.ExternalAPIs;

namespace API_DISCOVER
{
    [ExcludeFromCodeCoverage]
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;
        private bool _processRabbitReady = false;
        private bool _processRabbitDeleteReady = false;
        private bool _processDiscoverLoadedEntities = false;
        private bool _processRemoveBlankNodes = false;


        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Thread.Sleep(20000);
            _logger.LogInformation("Timed Hosted Service running.");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_processRabbitReady && !Discover.ProcessingItem)
                    {
                        var scope = _serviceScopeFactory.CreateScope();
                        RabbitMQService rabbitMQService = scope.ServiceProvider.GetRequiredService<RabbitMQService>();
                        Discover descubrimiento = new Discover(_serviceScopeFactory);
                        rabbitMQService.ListenToQueue(new RabbitMQService.ReceivedDelegate(descubrimiento.ProcessItem), new RabbitMQService.ShutDownDelegate(OnShutDown), rabbitMQService.queueName);
                        _processRabbitReady = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                    _processRabbitReady = false;
                }
                try
                {
                    if (!_processRabbitDeleteReady && !Discover.ProcessingDeletedItem)
                    {
                        var scope = _serviceScopeFactory.CreateScope();
                        RabbitMQService rabbitMQService = scope.ServiceProvider.GetRequiredService<RabbitMQService>();
                        Discover descubrimiento = new Discover(_serviceScopeFactory);
                        rabbitMQService.ListenToQueue(new RabbitMQService.ReceivedDelegate(descubrimiento.ProcessDeletedItem), new RabbitMQService.ShutDownDelegate(OnShutDownDelete), rabbitMQService.queueNameDelete);
                        _processRabbitDeleteReady = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                    _processRabbitDeleteReady = false;
                }
                try
                {
                    if (!_processDiscoverLoadedEntities)
                    {
                        new Thread(() =>
                        {
                            try
                            {
                                ConfigService ConfigService = new ConfigService();
                                //Todos los lunes a la 8:00 sería "0 0 8 ? * MON"
                                var expression = new CronExpression(ConfigService.GetLaunchDiscoverLoadedEntitiesCronExpression());
                                DateTimeOffset? time = expression.GetTimeAfter(DateTimeOffset.UtcNow);

                                if (time.HasValue)
                                {
                                    Thread.Sleep((time.Value.UtcDateTime - DateTimeOffset.UtcNow));
                                    Discover descubrimiento = new Discover(_serviceScopeFactory);
                                    descubrimiento.ApplyDiscoverLoadedEntities(ConfigService.GetSleepSecondsAfterProcessEntityDiscoverLoadedEntities(), _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallUrisFactoryApiService>());
                                    _processDiscoverLoadedEntities = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex);
                            }
                        }).Start();
                        _processDiscoverLoadedEntities = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                    _processDiscoverLoadedEntities = false;
                }
                try
                {
                    if (!_processRemoveBlankNodes)
                    {
                        new Thread(() =>
                        {
                            try
                            {
                                #region Cargamos configuraciones
                                ConfigSparql ConfigSparql = new ConfigSparql();
                                string SGI_SPARQLEndpoint = ConfigSparql.GetEndpoint();
                                string SGI_SPARQLGraph = ConfigSparql.GetGraph();
                                string SGI_SPARQLQueryParam = ConfigSparql.GetQueryParam();
                                string SGI_SPARQLUsername = ConfigSparql.GetUsername();
                                string SGI_SPARQLPassword = ConfigSparql.GetPassword();
                                #endregion
                                var expression = new CronExpression("0 0 * ? * *");
                                DateTimeOffset? time = expression.GetTimeAfter(DateTimeOffset.UtcNow);
                                CallUrisFactoryApiService callUrisFactoryApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallUrisFactoryApiService>();
                                HashSet<string> graphs = new HashSet<string>();
                                ORCID_API orcid_api = new ORCID_API();
                                CROSSREF_API crossref_api = new CROSSREF_API();
                                DBLP_API dblp_api = new DBLP_API();
                                DBPEDIA_API dbpedia_api = new DBPEDIA_API();
                                DOAJ_API doaj_api = new DOAJ_API();
                                PUBMED_API pubmed_api = new PUBMED_API();
                                RECOLECTA_API recolecta_api = new RECOLECTA_API();
                                SCOPUS_API scopus_api = new SCOPUS_API();
                                WOS_API wos_api = new WOS_API();
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", orcid_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", crossref_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", dblp_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", dbpedia_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", doaj_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", pubmed_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", recolecta_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", scopus_api.Id));
                                graphs.Add(callUrisFactoryApiService.GetUri("Graph", wos_api.Id));
                                graphs.Add(SGI_SPARQLGraph);

                                if (time.HasValue)
                                {
                                    Thread.Sleep((time.Value.UtcDateTime - DateTimeOffset.UtcNow));

                                    AsioPublication asioPublication = new AsioPublication(SGI_SPARQLEndpoint, SGI_SPARQLQueryParam, SGI_SPARQLGraph, SGI_SPARQLUsername, SGI_SPARQLPassword, _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<RabbitMQService>());
                                    foreach (string graph in graphs)
                                    {
                                        asioPublication.DeleteOrphanNodes(new HashSet<string>() { graph });
                                    }
                                    _processRemoveBlankNodes = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex);
                            }
                        }).Start();
                        _processRemoveBlankNodes = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                    _processRemoveBlankNodes = false;
                }
            }
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }

        private void OnShutDown()
        {
            _processRabbitReady = false;
        }

        private void OnShutDownDelete()
        {
            _processRabbitDeleteReady = false;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
