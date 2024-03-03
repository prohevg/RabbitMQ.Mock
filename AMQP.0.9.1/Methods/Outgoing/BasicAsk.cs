using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class BasicAsk : IOutgoingAmqpMethod
    {
        public BasicAsk(LongLong delivaryTag)
        {
            DeliveryTag = delivaryTag;
        }

        public LongLong DeliveryTag { get; set; }

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
