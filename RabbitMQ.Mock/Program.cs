//using AMQP_0_9_1.Transport;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Host;

var address = new Address("amqp://localhost:5673");

// uncomment the following to write frame traces
//AmqpTrace.TraceLevel = AmqpTraceLevel.Frame;
//AmqpTrace.TraceListener = (l, f, a) => Console.WriteLine(DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));

var host = new ContainerHost(address, cfg => cfg.UseMassTransit = true);
await host.OpenAsync();

Console.WriteLine("Host is listening on {0}:{1}", address.Host, address.Port);
Console.WriteLine("Press enter key to exit...");
Console.ReadLine();

await host.CloseAsync();