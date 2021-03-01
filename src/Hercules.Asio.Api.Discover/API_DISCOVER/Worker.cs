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

namespace API_DISCOVER
{
    [ExcludeFromCodeCoverage]
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;
        private bool _processRabbitReady = false;
        private bool _processDiscoverLoadedEntities = false;


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
                    if (!_processRabbitReady)
                    {
                        var scope = _serviceScopeFactory.CreateScope();
                        RabbitMQService rabbitMQService = scope.ServiceProvider.GetRequiredService<RabbitMQService>();
                        Discover descubrimiento = new Discover(_logger,_serviceScopeFactory);
                        rabbitMQService.ListenToQueue(new RabbitMQService.ReceivedDelegate(descubrimiento.ProcessItem), new RabbitMQService.ShutDownDelegate(OnShutDown));
                        _processRabbitReady = true;
                    }
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
                                    Thread.Sleep((time.Value.DateTime - DateTimeOffset.UtcNow));
                                    Discover descubrimiento = new Discover(_logger, _serviceScopeFactory);
                                    descubrimiento.ApplyDiscoverLoadedEntities(ConfigService.GetSleepSecondsAfterProcessEntityDiscoverLoadedEntities());
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
                }
                Thread.Sleep(1000);
            }
            return Task.CompletedTask;
        }

        private void OnShutDown()
        {
            _processRabbitReady = false;
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
