using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Mock;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    internal class Exchange : IExchange
    {
        #region fields

        private readonly ConcurrentDictionary<string, string> _queueBind = new();
        private readonly IQueueList _queueList;
        private readonly IMockCaseListHandler _mockCaseHandler;

        #endregion

        public Exchange(string name, IQueueList queueList, IMockCaseListHandler mockCaseHandler)
        {
            Name = name;
            _queueList = queueList;
            _mockCaseHandler = mockCaseHandler;
        }

        #region IEchange

        public string Name { get; }

        public void BindQueue(string queue, string routingKey)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"BindQueue exchange='{Name}' for queue='{queue}' with routing key='{routingKey}'");

            _queueBind.AddOrUpdate(queue, key => routingKey, (key, value) => routingKey);
        }

        public async ValueTask RouteToAsync(string routingKey, LinkedList<IAmqpFrameContent> content)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"RouteTo exchange='{Name}' with routing key='{routingKey}'");

            if (!_queueBind.Any())
            {
                return;
            }

            if (_queueBind.TryGetValue(routingKey, out var queueName))
            {
                if (_queueList.TryGetValue(queueName, out var queue))
                {
                    var message = GetMessage(content);
                    var input = Encoding.UTF8.GetString(message);
                    var response = await GetMockResponseAsync(input).ConfigureAwait(false);
                    queue.RouteTo(Name, response);
                }
            }
        }

        #endregion

        #region private

        private byte[] GetMessage(LinkedList<IAmqpFrameContent> content)
        {
            var size = GetMessageSize(content);
            var buffer = new byte[size];

            var offset = 0;
            foreach (var item in content)
            {
                Array.Copy(item.Payload.Buffer, 0, buffer, offset, item.PayloadLength);
                offset += item.Payload.Buffer.Length;
            }

            return buffer;
        }

        private int GetMessageSize(LinkedList<IAmqpFrameContent> content)
        {
            return content.Sum(x => x.PayloadLength);
        }

        private async ValueTask<byte[]> GetMockResponseAsync(string message)
        {
            if (message == null)
            {
                return null;
            }

            var response = await _mockCaseHandler.GetResponseAsync(message).ConfigureAwait(false);
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(response);
        }

        #endregion
    }
}