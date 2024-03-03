using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class QueueBindOk : IOutgoingAmqpMethod
    {
        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Queue);

        public Short MethodId => Short.Create(QueueMethodConstants.BindOk);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IOutgoingAmqpMethod

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
