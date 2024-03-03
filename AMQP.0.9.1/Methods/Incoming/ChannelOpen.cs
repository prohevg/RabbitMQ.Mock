using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using System;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ChannelOpen : IIncomingAmqpMethod
    {
        public Octet Reserved1 { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(20);

        public Short MethodId => Short.Create(10);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            //bufferSize += Reserved1.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            Reserved1 = Octet.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
