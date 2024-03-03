using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class ChannelOpenOk : IOutgoingAmqpMethod
    {
        public Longstr Reserved1 { get; set; } = Longstr.Empty;

        #region IOutgoingAmqpMethod

        public Short ClassId => Short.Create(20);

        public Short MethodId => Short.Create(11);

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            Reserved1.WriteTo(buffer);
            return buffer.Length - length;
        }

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion
    }
}
