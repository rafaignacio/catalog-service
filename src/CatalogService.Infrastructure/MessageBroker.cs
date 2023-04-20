using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CatalogService.Infrastructure;

public class MessageBroker
{
    private string _connectionString;

    public MessageBroker(string connectionString) =>
        _connectionString = connectionString;

    public void SendMessage<T>(string queueName, string eventName, T @event)
    {
        var connectionFactory = new ConnectionFactory()
        {
            HostName = _connectionString,
        };
        using var conn = connectionFactory.CreateConnection();
        using var channel = conn.CreateModel();
        channel.ExchangeDeclare(eventName, ExchangeType.Direct, true);
        channel.QueueDeclare(queueName, 
            exclusive: false,
            durable: true,
            autoDelete: false);
        channel.QueueBind(queueName, eventName, "", null);

        var jsonBody = JsonSerializer.Serialize(@event);

        channel.BasicPublish(exchange: eventName,
            mandatory: true,
            basicProperties: null,
            routingKey: "",
            body: Encoding.UTF8.GetBytes(jsonBody));
    }
}
