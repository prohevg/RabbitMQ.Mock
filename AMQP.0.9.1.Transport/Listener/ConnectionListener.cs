using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Factories;
using AMQP_0_9_1.Transport.Holders;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Listener
{
    /// <summary>
    /// The connection listener accepts AMQP connections from an address.
    /// </summary>
    public class ConnectionListener : IConnectionListener
    {
        #region private

        private readonly Address _address;
        private readonly ITransportListenerFactory _listenerFactory;
        private readonly IConnectionFactory _connectionFactory;
        private ITransportListener _listener;
        private IConnectionList _connections;

        #endregion

        /// <summary>
        /// Initializes the connection listener object.
        /// </summary>
        /// <param name="address">Address listen</param>
        /// <param name="listenerFactory">Transport listener factory</param>
        /// <param name="connectionFactory">Connection factory</param>
        /// <param name="connections">Connections</param>
        public ConnectionListener(Address address, 
            ITransportListenerFactory listenerFactory, 
            IConnectionFactory connectionFactory,
            IConnectionList connections)
        {
            _address = address;
            _listenerFactory = listenerFactory;
            _connectionFactory = connectionFactory;
            _connections = connections;
        }

        public async Task OpenAsync(CancellationToken token)
        {
            _listener = _listenerFactory.Create(_address);
            _listener.AcceptClient += Listener_AcceptClient;

            await _listener.OpenAsync(token).ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            _listener.AcceptClient -= Listener_AcceptClient;

            await _listener.CloseAsync();

            foreach (var connection in _connections.List)
            {
                await connection.CloseAsync().ConfigureAwait(false);
            }
        }

        #region private

        private async void Listener_AcceptClient(object sender, AcceptClientEventArgs args)
        {
            var connection = _connectionFactory.Create(args.AsyncTransport);
            connection.Close += Connection_CloseClient;

            await connection.OpenAsync().ConfigureAwait(false);

            _connections.Add(connection.Id, connection);
        }

        private async void Connection_CloseClient(object sender, ConnectionCloseEventArgs args)
        {
            if (_connections.TryGetValue(args.ConnectionId, out var connection))
            {
                connection.Close -= Connection_CloseClient;
                await connection.CloseAsync();
                connection.Dispose();

                _connections.Remove(args.ConnectionId);
            }
        }

        #endregion
    }
}
