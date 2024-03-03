using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    public class ConnectionOpenOk : IOutgoingAmqpMethod
    {
        public Octet Reserved1 { get; set; } = Octet.Empty;

        #region IAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(41);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            return Long.Create(bufferSize);
        }

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            Reserved1.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
