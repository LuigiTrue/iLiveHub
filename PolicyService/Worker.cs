using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PolicyService.Application.Services;
using PolicyService.Domain.Entities;
using System.Text;
using System.Text.Json;

namespace PolicyService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: "Policy", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += ProcessMessageAsync;
            await _channel.BasicConsumeAsync(queue: "Policy", autoAck: true, consumer: consumer);
        }

        private async Task ProcessMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            using var scope = _scopeFactory.CreateScope();
            var statementObject = ProcessReceivedMessage(ea.Body.ToArray());
            var statementService = scope.ServiceProvider.GetRequiredService<IStatementService>();

            await ExecuteServiceRequired(statementObject.Item2, statementService);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public (Statement?, Message) ProcessReceivedMessage(byte[] bytesMessage)
        {
            try
            {
                var jsonMessage = Encoding.UTF8.GetString(bytesMessage);
                var message = JsonSerializer.Deserialize<Message>(jsonMessage);
                var objectMessage = message.Object;
                return (objectMessage, message);
            }
            catch
            {
                //TODO: AQUI A MENSAGEM DEVE SER DEVOLVIDA PARA O APP
                var message = new Message();
                var statement = new Statement();
                return (statement, message);
            }

        }

        public async Task ExecuteServiceRequired(Message? message, IStatementService statementService)
        {
            if (message.IsAuthenticaded)
            {
                var statement = message.Object;

                switch (message.ServiceID)
                {
                    case 1: await statementService.GetAllStatements(); break;
                    case 2: await statementService.AddStatement(statement.Status, statement.StatementType, statement.ActiveTime, statement.SectorId, statement.ReceiverType, statement.Title, statement.Content); break;
                    case 3: await statementService.GetStatementById(message.StatementID); break;
                    case 4: await statementService.DeleteStatement(message.StatementID); break;
                    case 5: await statementService.UpdateStatement(statement); break;
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }

}