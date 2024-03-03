using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ConnectionStartOk : IIncomingAmqpMethod
    {
        public Table ClientProperties { get; set; }
        public Shortstr Mechanism { get; set; }
        public Longstr Response { get; set; }
        public Shortstr Locale { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(11);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ClientProperties.SizeOf();
            bufferSize += Mechanism.SizeOf();
            bufferSize += Response.SizeOf();
            bufferSize += Locale.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            ClientProperties = Table.Create(buffer);
            Mechanism = Shortstr.Create(buffer);
            Response = Longstr.Create(buffer);
            Locale = Shortstr.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
