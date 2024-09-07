using OT.Assessment.Infrastructure.Interfaces;
using OT.Assessment.Infrastructure.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Consumer.Workers
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var rmqConsumer = scope.ServiceProvider.GetRequiredService<IRMQConsumer>();

                             rmqConsumer.Consume();
                        }

                        await Task.Delay(5000, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while consuming RabbitMQ messages.");
                    }
                }

                await Task.CompletedTask;

                await Task.Delay(5000, stoppingToken);  
            }
        }
    }
}
