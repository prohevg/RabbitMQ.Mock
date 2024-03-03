using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Holders;

namespace AMQP_0_9_1.Transport.Factories
{
    public class ChannelFactory : IChannelFactory
    {
        private readonly IAmqpMethodFactory _methodFactory;
        private readonly IExchangeFactory _exchangeFactory;
        private readonly IExchangeList _exchanges;
        private readonly IQueueFactory _queueFactory;
        private readonly IQueueList _queue;

        public ChannelFactory(IAmqpMethodFactory methodFactory,
            IExchangeFactory exchangeFactory,
            IExchangeList exchanges,
            IQueueFactory queueFactory,
            IQueueList queue)
        {
            _methodFactory = methodFactory;
            _exchangeFactory = exchangeFactory;
            _exchanges = exchanges;
            _queueFactory = queueFactory;
            _queue = queue;
        }

        public IChannel Create(int number, IConnection connection)
        {
            return new Channel((ushort)number, 
                connection,
                _methodFactory,
                _exchangeFactory,
                _exchanges,
                _queueFactory,
                _queue);
        }
    }
}