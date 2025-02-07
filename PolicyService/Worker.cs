using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PolicyService.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PolicyService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var statementService = scope.ServiceProvider.GetRequiredService<IStatementService>();
                        var listTest = await statementService.GetStatement();

                        foreach (var item in listTest)
                        {
                            Console.WriteLine(item.Title.ToString());
                        }
                    }
                };

                await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}