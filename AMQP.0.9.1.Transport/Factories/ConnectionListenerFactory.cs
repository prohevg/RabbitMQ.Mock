using AMQP_0_9_1.Listener;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Holders;

namespace AMQP_0_9_1.Transport.Factories
{
    public class ConnectionListenerFactory : IConnectionListenerFactory
    {
        private readonly ITransportListenerFactory _listenerFactory;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnectionList _connectionList;

        public ConnectionListenerFactory(ITransportListenerFactory listenerFactory, 
            IConnectionFactory connectionFactory,
            IConnectionList connectionList)
        {
            _listenerFactory = listenerFactory;
            _connectionFactory = connectionFactory;
            _connectionList = connectionList;
        }

        public IConnectionListener Create(Address address)
        {
            return new ConnectionListener(address, _listenerFactory, _connectionFactory, _connectionList);
        }
    }
}
