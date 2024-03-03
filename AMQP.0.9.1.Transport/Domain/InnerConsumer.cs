using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Methods.Outgoing;
using AMQP_0_9_1.Transport;
using AMQP_0_9_1.Transport.Contexts;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Types;
using System;
using System.Collections.Generic;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Domain
{
    public class InnerConsumer
    {
        private readonly ushort _channelId;
        private readonly string _consumerTag;
        private readonly IConnection _connection;

        public ushort ChannelId  => _channelId;
        public string ConsumerTag => _consumerTag;

        public InnerConsumer(ushort channelId, string consumerTag, IConnection connection)
        {
            this._channelId = channelId;
            this._consumerTag = consumerTag;
            this._connection = connection;
        }

        public void Send(string exchange, string queue, byte[] message)
        {
            var context = CreateSendContext(_channelId, exchange, queue, message);

            _connection.SendFrames(_channelId, context);
        }

        #region publish

        private ISendFrameContext CreateSendContext(ushort channelId, string exchange, string queue, byte[] content)
        {
            try
            {
                var frameMethod = CreateMethod(channelId, exchange, queue);
                var frameContent = CreateContent(channelId, content);
                var frameHeader = CreateHeader(channelId, frameContent);

                var list = new LinkedList<IAmqpFrameContent>();
                list.AddLast(frameContent);

                return new SendFrameContext(frameMethod, frameHeader, list);
            }
            catch (Exception ex)
            {
                AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.Message);
                return null;
            }
        }

        private IAmqpFrameMethod CreateMethod(ushort channelId, string exchange, string queue)
        {
            var deliver = new BasicDeliver
            {
                Exchange = exchange,
                RoutingKey = queue,
                ConsumerTag = Shortstr.Create("basicConsume.ConsumerTag"), 
                DeliveryTag = LongLong.Create(0),
                Redelivered = Boolean.Create(false)
            };

            var frame = new AmqpFrameMethod
            {
                FrameType = Octet.Create((byte)FrameType.FrameMethod),
                ChannelId = Short.Create(channelId),
            };

            frame.Payload = new ByteBuffer(0);

            deliver.WriteTo(frame.Payload);

            return frame;
        }

        private IAmqpFrameContentHeader CreateHeader(ushort channelId, IAmqpFrameContent content)
        {

            var basicProperties = new BasicProperties();

            IAmqpFrameContentHeader frame = new AmqpFrameContentHeader
            {
                FrameType = (int)FrameType.FrameHeader,
                ChannelId = Short.Create(channelId),
                ClassId = Short.Create(ClassConstants.Basic),
                BasicProperties = basicProperties,
                Weight = Short.Create(0),
                BodySize = LongLong.Create(content.Payload.Length)
            };

            return frame;
        }

        private IAmqpFrameContent CreateContent(ushort channelId, byte[] content)
        {
            IAmqpFrameContent frame = new AmqpFrameContent
            {
                FrameType = (int)FrameType.FrameBody,
                ChannelId = Short.Create(channelId),
                PayloadLength = content.Length,
                Payload = new ByteBuffer(content.Length, false)
            };

            frame.Payload.WriteBytes(content, 0, content.Length);

            return frame;
        }

        #endregion
    }
}
