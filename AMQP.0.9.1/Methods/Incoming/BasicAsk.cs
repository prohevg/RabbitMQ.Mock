using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class FromClientBasicAsk : IIncomingAmqpMethod
    {
        public LongLong DeliveryTag { get; set; } = LongLong.Empty;

        public Octet Multiple { get; set; } = Octet.Empty;

        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Basic);

        public Short MethodId => Short.Create(BasicMethodConstants.Ack);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += DeliveryTag.SizeOf();
            bufferSize += Multiple.SizeOf();
            return Long.Create(bufferSize);
        }

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            DeliveryTag = LongLong.Create(buffer);
            Multiple = Octet.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion

        #region IOutgoingAmqpMethod

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            DeliveryTag.WriteTo(buffer);
            Multiple.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
