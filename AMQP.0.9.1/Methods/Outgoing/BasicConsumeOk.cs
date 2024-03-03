using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class BasicConsumeOk : IOutgoingAmqpMethod
    {
        public BasicConsumeOk(Shortstr consumerTag)
        {
            ConsumerTag = consumerTag;
        }

        public Shortstr ConsumerTag { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Basic);

        public Short MethodId => Short.Create(BasicMethodConstants.ConsumeOk);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ConsumerTag.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IOutgoingAmqpMethod

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            ConsumerTag.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
