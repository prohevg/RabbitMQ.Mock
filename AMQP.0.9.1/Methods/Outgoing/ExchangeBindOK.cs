using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class ExchangeBindOK : IOutgoingAmqpMethod
    {
        #region IOutgoingAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Exchange);

        public Short MethodId => Short.Create(ExchangeMethodConstants.BindOk);

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            return buffer.Length - length;
        }

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion
    }
}
