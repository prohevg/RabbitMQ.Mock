using AMQP_0_9_1.Methods.Incoming;
using AMQP_0_9_1.Methods.Outgoing;
using AMQP_0_9_1.Transport.Contexts;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Factories;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Domain
{
    public class Channel : IChannel
    {
        #region fields

        private readonly IConnection _connection;
        private readonly IAmqpMethodFactory _methodFactory;
        private readonly IExchangeFactory _exchangeFactory;
        private readonly IExchangeList _exchanges;
        private readonly IQueueFactory _queueFactory;
        private readonly IQueueList _queue;
        private readonly LinkedList<string> _consumers = new();

        #endregion

        #region Constructor

        public Channel(ushort id, 
            IConnection connection, 
            IAmqpMethodFactory methodFactory,
            IExchangeFactory exchangeFactory,
            IExchangeList exchanges,
            IQueueFactory queueFactory,
            IQueueList queue)
        {
            Id = id;
            _connection = connection;
            _methodFactory = methodFactory;
            _exchangeFactory = exchangeFactory;
            _exchanges = exchanges;
            _queueFactory = queueFactory;
            _queue = queue;
        }

        #endregion

        #region IChannel

        /// <summary>
        /// Channel id
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        /// OpenAsync
        /// </summary>
        public ValueTask OpenAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// CloseAsync
        /// </summary>
        public ValueTask CloseAsync()
        {
            foreach (var tagName in _consumers)
            {
                foreach (var queue in _queue.List)
                {
                    queue.RemoveConsumer(Id, tagName);
                }
            }

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Receive frame
        /// </summary>
        public ValueTask<bool> OnFrameAsync(IReceiveFrameContext context)
        {
            var amqpMethod = _methodFactory.CreateIncomingMethod(context.Method);
            if (amqpMethod is ConfirmSelect)
            {
                _connection.SendMethod(Id, new ConfirmSelectOk());
                return ValueTask.FromResult(true);
            }

            if (amqpMethod is ExchangeDeclare exchangeDeclare)
            {
                ExchangeDeclare(exchangeDeclare);
                return ValueTask.FromResult(true);
            }

            if (amqpMethod is QueueDeclare queueDeclare)
            {
                QueueDeclare(queueDeclare);
                return ValueTask.FromResult(true);
            }
            else if (amqpMethod is QueueBind queueBind)
            {
                QueueBind(queueBind);
                return ValueTask.FromResult(true);
            }

            if (amqpMethod is BasicQos)
            {
                _connection.SendMethod(Id, new BasicQosOk());
                return ValueTask.FromResult(true);
            }

            if (amqpMethod is BasicConsume basicConsume)
            {
                BasicConsume(basicConsume);
                return ValueTask.FromResult(true);
            }

            if (amqpMethod is BasicPublish basicPublish)
            {
                BasicPublish(context, basicPublish);
                return ValueTask.FromResult(true);
            }

            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }

        #endregion

        #region private

        private void ExchangeDeclare(ExchangeDeclare exchangeDeclare)
        {
            var newExchange = _exchangeFactory.Create(exchangeDeclare.Exchange);

            _exchanges.Add(newExchange.Name, newExchange);

            _connection.SendMethod(Id, new ExchangeDeclareOK());
        }

        private void QueueDeclare(QueueDeclare queueDeclare)
        {
            var newQueue = _queueFactory.Create(queueDeclare.Queue);

            _queue.Add(newQueue.Name, newQueue);

            _connection.SendMethod(Id, new QueueDeclareOk(newQueue.Name, 0, 0));
        }

        private void QueueBind(QueueBind queueBind)
        {
            if (_exchanges.TryGetValue(queueBind.Exchange, out var exchange))
            {
                exchange.BindQueue(queueBind.Queue, queueBind.RoutingKey);
            }
            else
            {
                var newExchange = _exchangeFactory.Create(queueBind.Exchange);

                _exchanges.Add(newExchange.Name, newExchange);

                newExchange.BindQueue(queueBind.Queue, queueBind.RoutingKey);
            }

            _connection.SendMethod(Id, new QueueBindOk());
        }

        private void BasicPublish(IReceiveFrameContext context, BasicPublish basicPublish)
        {
            if (!_exchanges.TryGetValue(basicPublish.Exchange, out var exchange))
            {
                exchange = _exchangeFactory.Create(basicPublish.Exchange);
                exchange.BindQueue(basicPublish.RoutingKey, basicPublish.RoutingKey);

                _exchanges.Add(exchange.Name, exchange);
            }

            exchange.RouteToAsync(basicPublish.RoutingKey, context.Content).ConfigureAwait(false);

            if (context.Header.BasicProperties?.Headers?.TryGetValue("publishId", out var publishId) ?? false)
            {
                _connection.SendMethod(Id, new BasicAsk(ulong.Parse(publishId.ToString())));
            }
            else
            {
                _connection.SendMethod(Id, new BasicAsk(LongLong.Create(1)));
            }
        }

        private void BasicConsume(BasicConsume basicConsume)
        {
            var tagName = string.IsNullOrWhiteSpace(basicConsume.ConsumerTag)
                ? Shortstr.Create("basicConsume.ConsumerTag")
                : basicConsume.ConsumerTag;

            _consumers.AddFirst(tagName);

            if (_queue.TryGetValue(basicConsume.Queue, out var queue))
            {
                queue.AddConsumer(basicConsume.ConsumerTag, new InnerConsumer(Id, tagName, _connection));
            }

            _connection.SendMethod(Id, new BasicConsumeOk(tagName));
        }

        #endregion
    }
}
