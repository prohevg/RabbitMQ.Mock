using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Mock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    internal class MasstransitExchange : IExchange
    {
        #region fields

        private readonly ConcurrentDictionary<string, string> _queueBind = new();
        private readonly IQueueList _queueList;
        private readonly IMockCaseListHandler _mockCaseHandler;

        #endregion

        public MasstransitExchange(string name, IQueueList queueList, IMockCaseListHandler mockCaseHandler)
        {
            Name = name;
            _queueList = queueList;
            _mockCaseHandler = mockCaseHandler;
        }

        #region IExchange

        public string Name { get; }

        public void BindQueue(string queue, string routingKey)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"BindQueue exchange='{Name}' for queue='{queue}' with routing key='{routingKey}'");

            _queueBind.AddOrUpdate(queue, key => routingKey, (key, value) => routingKey);
        }

        public async ValueTask RouteToAsync(string routingKey, LinkedList<IAmqpFrameContent> content)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"RouteTo exchange='{Name}' with routing key='{routingKey}'");

            var message = GetMessage(content);
            var input = Deserialize(message);

            if (!string.IsNullOrEmpty(input.ResponseAddress))
            {
                var response = await CreateResponseAsync(input).ConfigureAwait(false);

                var tempQueueName = new Uri(input.ResponseAddress).AbsolutePath.TrimStart('/');
                if (_queueList.TryGetValue(tempQueueName, out var queueTemp))
                {
                    queueTemp.RouteTo(Name, response);
                }
            }

            foreach (var item in _queueBind)
            {
                if (item.Value == routingKey)
                {
                    if (_queueList.TryGetValue(routingKey, out var queue))
                    {
                        queue.RouteTo(Name, message);
                    }
                }
            }
        }

        private static MockMessageEnvelop Deserialize(byte[] message)
        {
            using var ms = new MemoryStream(message);
            using var reader = new StreamReader(ms, Encoding.UTF8);
            return JsonSerializer.Create().Deserialize(reader, typeof(MockMessageEnvelop)) as MockMessageEnvelop;
        }

        private async ValueTask<byte[]> CreateResponseAsync(MockMessageEnvelop input)
        {
            var types = JsonConvert.DeserializeObject<List<string>>(input.Headers["MT-Request-AcceptType"].ToString());

            var response = new MockMessageEnvelop
            {
                CorrelationId = input.CorrelationId,
                ConversationId = input.ConversationId,
                RequestId = input.RequestId,
                MessageId = Guid.NewGuid().ToString(),
                Message = await GetMockResponseAsync(input.Message).ConfigureAwait(false),
                MessageType = types.ToArray(),
            };

            var s = JsonConvert.SerializeObject(response);
            return Encoding.UTF8.GetBytes(s);
        }

        private async ValueTask<object> GetMockResponseAsync(object message)
        {
            if (message == null)
            {
                return null;
            }

            var jObject = JObject.FromObject(message);
            var response = await _mockCaseHandler.GetResponseAsync(jObject.ToString()).ConfigureAwait(false);
            return JObject.Parse(response);
        }

        private byte[] GetMessage(LinkedList<IAmqpFrameContent> content)
        {
            var size = GetMessageSize(content);
            var buffer = new byte[size];

            var offset = 0;
            foreach (var item in content)
            {
                try
                {
                    Array.Copy(item.Payload.Buffer, 0, buffer, offset, item.PayloadLength);
                    offset += item.Payload.Buffer.Length;
                }
                catch (Exception ex)
                {
                    AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.Message);
                    throw;
                }
            }

            return buffer;
        }

        private int GetMessageSize(LinkedList<IAmqpFrameContent> content)
        {
            return content.Sum(x => x.Payload.Length);
        }

        #endregion
    }

    /// <summary>
    /// MessageEnvelop
    /// </summary>
    public class MockMessageEnvelop
    {
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string RequestId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string[] MessageType { get; set; }
        public object Message { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? SentTime { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        //public HostInfo? Host { get; set; }
    }
}