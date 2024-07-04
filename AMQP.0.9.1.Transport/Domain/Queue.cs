using AMQP_0_9_1.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    public class Queue : IQueue
    {
        private readonly LinkedList<byte[]> _messages = new();
        private readonly LinkedList<InnerConsumer> _consumers = new();

        public Queue(string name) 
        { 
            Name = name;
        }

        #region IQueue

        public string Name { get; }

        public void AddConsumer(string consumerTag, InnerConsumer innerConsumer)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"Queue='{Name}' add consumer='{consumerTag}'");

            _consumers.AddLast(innerConsumer);            
        }

        public void RemoveConsumer(ushort channelId, string consumerTag)
        {
            var consumer = _consumers
                .FirstOrDefault(x => x.ChannelId == channelId
                                     && 
                                     x.ConsumerTag == consumerTag);

            if (consumer != null)
            {
                try
                {
                    _consumers.Remove(consumer);
                }
                catch (System.Exception ex)
                {
                    AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.Message);
                }
            }
        }

        public void RouteTo(string exchange, byte[] message)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"Queue='{Name}' route message='{message.Length}'");
            
            _messages.AddLast(message);
        }

        public async Task HandleAsync(CancellationToken token)
        {
            if (!_consumers.Any())
            {
                return;
            }

            foreach (var message in _messages.ToList())
            {
                foreach (var consumer in _consumers)
                {
                    consumer.Send("", Name, message);
                    break;
                }

                _messages.RemoveFirst();

                await Task.Delay(100);
            }
        }

        #endregion
    }
}
