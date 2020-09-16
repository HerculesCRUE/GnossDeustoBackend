using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API_DISCOVER.Models;
using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Log;
using API_DISCOVER.Models.Services;
using API_DISCOVER.Models.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;

namespace API_DISCOVER
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;
        private bool _processReady = false;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_processReady)
                    {
                        var scope = _serviceScopeFactory.CreateScope();
                        RabbitMQService rabbitMQService = scope.ServiceProvider.GetRequiredService<RabbitMQService>();
                        rabbitMQService.ListenToQueue(new RabbitMQService.ReceivedDelegate(ProcessItem), new RabbitMQService.ShutDownDelegate(OnShutDown));
                        _processReady = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                Thread.Sleep(1000);
            }
            return Task.CompletedTask;
        }

        private void OnShutDown()
        {
            _processReady = false;
        }

        private bool ProcessItem(string item)
        {
            DiscoverItem discoverItem = JsonConvert.DeserializeObject<DiscoverItem>(item);
            try
            {
                //Aplicamos el proceso de descubrimiento
                DiscoverResult resultado = Discover.Init(discoverItem.Rdf, discoverItem.DissambiguationProcessed);
                Discover.Process(discoverItem, resultado, _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>());
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                //Se ha producido un error al aplicar el descubrimiento
                DiscoverItemBDService pDiscoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();

                DiscoverItem discoverItemBBDD = null;
                if (discoverItem.ID != Guid.Empty)
                {
                    discoverItemBBDD = pDiscoverItemBDService.GetDiscoverItemById(discoverItem.ID);
                }
                if (discoverItemBBDD != null)
                {
                    //Si existe el item en BBDD se actualiza
                    discoverItemBBDD.Status = DiscoverItem.DiscoverItemStatus.Error.ToString();
                    discoverItemBBDD.Error = $"{ex.Message}\n{ex.StackTrace}\n";
                    pDiscoverItemBDService.ModifyDiscoverItem(discoverItemBBDD);
                }
                else
                {
                    //Si no existe se crea
                    discoverItemBBDD = discoverItem;
                    discoverItemBBDD.Status = DiscoverItem.DiscoverItemStatus.Error.ToString();
                    discoverItemBBDD.Error = $"{ex.Message}\n{ex.StackTrace}\n";
                    pDiscoverItemBDService.AddDiscoverItem(discoverItemBBDD);
                }
            }
            return true;

        }


        public Task StopAsync(CancellationToken stoppingToken)
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
