
using System.Text;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using RabbitMQ.Client;

namespace SenderService;

public class Sender
{
    public readonly IConfiguration _configuration;

    public Sender(IConfiguration configuration){
        _configuration = configuration;
    }

    public async Task Send(string? jsonContent)
    {
        var factory = new ConnectionFactory {
            HostName = _configuration["RabbitMq:HostName"],
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"]
        };;
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "Policy", durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(jsonContent);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "Policy", body: body);
        Console.WriteLine($" [x] Sent {jsonContent}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

}