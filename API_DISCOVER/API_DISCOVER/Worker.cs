using System;
using System.Threading;
using System.Threading.Tasks;
using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Log;
using API_DISCOVER.Models.Services;
using API_DISCOVER.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                Discover.Process(discoverItem, resultado, 
                    _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>(),
                    _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallCronApiService>(),
                    _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>()
                    );
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                //Se ha producido un error al aplicar el descubrimiento
                //Modificamos los datos del DiscoverItem que ha fallado
                DiscoverItemBDService discoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();
                DiscoverItem discoverItemBBDD = discoverItemBDService.GetDiscoverItemById(discoverItem.ID);
                discoverItemBBDD.UpdateError($"{ex.Message}\n{ex.StackTrace}\n", discoverItem.Rdf);
                discoverItemBDService.ModifyDiscoverItem(discoverItemBBDD);

                if (!string.IsNullOrEmpty(discoverItem.JobID))
                {
                    //Si viene de una tarea actualizamos su estado de descubrimiento
                    ProcessDiscoverStateJobBDService processDiscoverStateJobBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>();
                    ProcessDiscoverStateJob processDiscoverStateJob = processDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(discoverItem.JobID);
                    if (processDiscoverStateJob != null)
                    {
                        processDiscoverStateJob.State = "Error";
                        processDiscoverStateJobBDService.ModifyProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                    else
                    {
                        processDiscoverStateJob = new ProcessDiscoverStateJob() { State = "Error", JobId = discoverItem.JobID };
                        processDiscoverStateJobBDService.AddProcessDiscoverStateJob(processDiscoverStateJob);
                    }
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
