using Newtonsoft.Json;
using OT.Assessment.Domain.Models;
using OT.Assessment.Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace OT.Assessment.Infrastructure.Messaging
{
    public class RMQConsumer : IRMQConsumer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Serilog.ILogger _logger;

        public RMQConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = Log.ForContext<RMQConsumer>();
        }

        public void Consume()
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "wagerQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.Information("Received message: {Message}", message);

                    try
                    {
                        var casinoWager = JsonConvert.DeserializeObject<CasinoWager>(message);

                        var player = await _unitOfWork.Players.GetPlayerAsync(casinoWager.AccountId);

                        if (player == null)
                        {
                            player = new Player { AccountId = casinoWager.AccountId, Username = casinoWager.Player.Username };
                            await _unitOfWork.Players.CreatePlayerAsync(player);
                        }

                        await _unitOfWork.CasinoWagers.CreateCasinoWagerAsync(casinoWager);
                        _logger.Information("Wager processed for AccountId: {AccountId}", casinoWager.AccountId);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error processing wager: {Message}", message);
                    }
                };

                channel.BasicConsume(queue: "wagerQueue", autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Error in RabbitMQ consumer");
            }
        }
    }
}
