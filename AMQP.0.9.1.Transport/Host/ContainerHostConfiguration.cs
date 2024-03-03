using AMQP_0_9_1.Transport.Settings;

namespace AMQP_0_9_1.Transport.Host
{
    public class ContainerHostConfiguration
    {
        public bool UseMassTransit { get; set; }

        public TcpSettings Tcp { get; set; } = new TcpSettings();

        public ConnectionSettings Connection { get; set; } = new ConnectionSettings();
    }
}
