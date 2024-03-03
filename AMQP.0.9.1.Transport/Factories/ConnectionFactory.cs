using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Settings;
using System.Threading.Channels;

namespace AMQP_0_9_1.Transport.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        #region fileds

        private readonly IAmqpMethodFactory _methodFactory;
        private readonly IChannelFactory _channelFactory;
        private readonly ConnectionSettings _settings;
        private readonly IChannelList _channels;

        #endregion

        public ConnectionFactory(IAmqpMethodFactory methodFactory,
            IChannelFactory channelFactory,
            IChannelList channels,
            ConnectionSettings settings)
        {
            _methodFactory = methodFactory;
            _channelFactory = channelFactory;
            _channels = channels;
            _settings = settings;
        }

        /// <summary>
        /// Create connection 
        /// </summary>
        /// <param name="transport">Transport</param>
        /// <returns>Connection</returns>
        public IConnection Create(IAsyncTransport transport)
        {
            return new Connection(transport,
                _methodFactory,
                _channelFactory,
                _channels,
                _settings);
        }
    }
}