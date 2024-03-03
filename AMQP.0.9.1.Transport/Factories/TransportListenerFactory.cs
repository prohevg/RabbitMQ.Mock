using AMQP_0_9_1.Listener;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Settings;

namespace AMQP_0_9_1.Transport.Factories
{
    public class TransportListenerFactory : ITransportListenerFactory
    {
        private readonly IAddressFactory _addressFactory;
        private readonly TcpSettings _tcpSettings;

        public TransportListenerFactory(IAddressFactory addressFactory, TcpSettings tcpSettings)
        {
            _addressFactory = addressFactory;
            _tcpSettings = tcpSettings;
        }

        public ITransportListener Create(Address address)
        {
            return new TcpTransportListener(_addressFactory, address.Host, address.Port, _tcpSettings);
        }
    }
}
