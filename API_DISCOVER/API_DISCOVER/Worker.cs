using API_DISCOVER.Models.Entities;
using API_DISCOVER.Models.Log;
using API_DISCOVER.Models.Services;
using API_DISCOVER.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace API_DISCOVER
{
    [ExcludeFromCodeCoverage]
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

        private bool ProcessItem(string itemIDstring)
        {
            Guid itemID = JsonConvert.DeserializeObject<Guid>(itemIDstring);            
            try
            {
               
                DiscoverItemBDService discoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();
                ProcessDiscoverStateJobBDService processDiscoverStateJobBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>();
                CallCronApiService callCronApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallCronApiService>();
                CallEtlApiService callEtlApiService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<CallEtlApiService>();

                DiscoverItem discoverItem = discoverItemBDService.GetDiscoverItemById(itemID);

                if (discoverItem != null)
                {
                    //Aplicamos el proceso de descubrimiento
                    DiscoverResult resultado = Discover.Init(discoverItem, callEtlApiService);
                    Discover.Process(discoverItem, resultado,
                        discoverItemBDService,
                        callCronApiService,
                        processDiscoverStateJobBDService
                        );
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                //Se ha producido un error al aplicar el descubrimiento
                //Modificamos los datos del DiscoverItem que ha fallado
                DiscoverItemBDService discoverItemBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<DiscoverItemBDService>();
                DiscoverItem discoverItemBBDD = discoverItemBDService.GetDiscoverItemById(itemID);
                discoverItemBBDD.UpdateError($"{ex.Message}\n{ex.StackTrace}\n");
                discoverItemBDService.ModifyDiscoverItem(discoverItemBBDD);

                if (!string.IsNullOrEmpty(discoverItemBBDD.JobID))
                {
                    //Si viene de una tarea actualizamos su estado de descubrimiento
                    ProcessDiscoverStateJobBDService processDiscoverStateJobBDService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ProcessDiscoverStateJobBDService>();
                    ProcessDiscoverStateJob processDiscoverStateJob = processDiscoverStateJobBDService.GetProcessDiscoverStateJobByIdJob(discoverItemBBDD.JobID);
                    if (processDiscoverStateJob != null)
                    {
                        processDiscoverStateJob.State = "Error";
                        processDiscoverStateJobBDService.ModifyProcessDiscoverStateJob(processDiscoverStateJob);
                    }
                    else
                    {
                        processDiscoverStateJob = new ProcessDiscoverStateJob() { State = "Error", JobId = discoverItemBBDD.JobID };
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
