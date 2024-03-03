using System.Text;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Host;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

try
{
    var address = new Address("amqp://localhost:5673");
    var host = new ContainerHost(address);
    await host.OpenAsync();

    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5673
    };

    var queueName = "SomeQueue";
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();

    channel.QueueDeclare(queue: queueName,
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

    var req = CreateRequest();
    var message = JsonConvert.SerializeObject(req);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "Some_Exchange",
                   routingKey: queueName,
                   body: body);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (ch, ea) =>
    {
        var content = Encoding.UTF8.GetString(ea.Body.ToArray());
        Console.Out.WriteLine($"Response from RabbitMQ.Mock: {content}");
        Console.Out.WriteLine("Done");
        channel.BasicAck(ea.DeliveryTag, false);
    };

    channel.BasicConsume(queueName, false, consumer);
}
catch (Exception ex)
{
    Console.Out.WriteLine(ex.ToString());
}

Console.In.ReadLine();

static IGetRequest CreateRequest()
{
    return new GetRequest
    {
        Name = "mock",
    };
}

internal interface IGetRequest
{
    public string Name { get; set; }
}

internal class GetRequest : IGetRequest
{
    public string Name { get; set; }
}
