using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ConnectionOpen : IIncomingAmqpMethod
    {
        public Shortstr VirtualHost { get; set; } = Shortstr.Create("/");

        public Octet Reserved1 { get; set; }
        public Octet Reserved2 { get; set; }


        #region IAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(40);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += VirtualHost.SizeOf();
            bufferSize += Reserved1.SizeOf();
            bufferSize += Reserved2.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            VirtualHost = Shortstr.Create(buffer);
            Reserved1 = Octet.Create(buffer);
            Reserved2 = Octet.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
