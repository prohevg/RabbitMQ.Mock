using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ConfirmSelect : IIncomingAmqpMethod
    {
        public Boolean NoWait { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(85);

        public Short MethodId => Short.Create(11);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            NoWait = Boolean.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
