using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Host;
using MassTransit;
using MasstransitRequestReplyConsole;
using Microsoft.Extensions.DependencyInjection;

try
{
    var address = new Address("amqp://localhost:5673");
    var host = new ContainerHost(address, cfg => cfg.UseMassTransit = true);
    await host.OpenAsync();

    var serviceProvider = FillServiceProvider();
    var busControl = serviceProvider.GetRequiredService<IBusControl>();
    await busControl.StartAsync();

    var client = serviceProvider.GetRequiredService<IRequestClient<IGetRequest>>();
    var response = await SendMessageAsync(client, TimeSpan.FromSeconds(20), "mock");
    
    Console.Out.WriteLine($"Response from RabbitMQ.Mock: {response.RespText}");
    Console.Out.WriteLine("Done");
}
catch (Exception ex)
{
    Console.Out.WriteLine(ex.ToString());
}

Console.In.ReadLine();

async Task<IGetResponse> SendMessageAsync(IRequestClient<IGetRequest> client, TimeSpan timeout, string? name = null)
{
    var cts = new CancellationTokenSource(timeout);

    var request = CreateRequest(name);

    try
    {
        var result = await client.GetResponse<IGetResponse>(request, cts.Token, timeout);
        return result.Message;
    }
    catch (Exception ex)
    {
        throw;
    }
}

IGetRequest CreateRequest(string name = null)
{
    return new GetRequest
    {
        Name = name ?? "mock"
    };
}

IServiceProvider FillServiceProvider()
{
    var serviceProvider = new ServiceCollection()
        .AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddRequestClient<IGetRequest>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;
                cfg.ConfigureEndpoints(context);
                cfg.Host("rabbitmq://localhost:5673");
            });
        })
        .BuildServiceProvider();


    return serviceProvider;
}

