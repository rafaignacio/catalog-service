using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CatalogService.Infrastructure;

public class MessageBroker
{
    private string _connectionString;
    private const short MaxRetries = 5;

    public MessageBroker(string connectionString) =>
        _connectionString = connectionString;

    public void SendMessage<T>(string queueName, string eventName, T @event, short retryNumber = 1)
    {
        if (retryNumber > MaxRetries)
            throw new TimeoutException($"Failed to deliver message after {MaxRetries} retries.");

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

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var jsonBody = JsonSerializer.Serialize(@event);

        channel.BasicPublish(exchange: eventName,
            mandatory: true,
            basicProperties: properties,
            routingKey: "",
            body: Encoding.UTF8.GetBytes(jsonBody));

        channel.ConfirmSelect();
        if(!channel.WaitForConfirms(TimeSpan.FromSeconds(5)))
        {
            SendMessage(queueName, eventName, @event, ++retryNumber);
        }
    }
}
