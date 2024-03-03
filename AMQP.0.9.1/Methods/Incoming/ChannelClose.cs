using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ChannelClose : IIncomingAmqpMethod
    {
        public Short ReplyCode { get; set; }
        public Shortstr ReplyText { get; set; }
        public Short ClassId1 { get; set; }
        public Short MethodId1 { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(20);

        public Short MethodId => Short.Create(40);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ReplyCode.SizeOf();
            bufferSize += ReplyText.SizeOf();
            bufferSize += ClassId1.SizeOf();
            bufferSize += MethodId1.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            ReplyCode = Short.Create(buffer);
            ReplyText = Shortstr.Create(buffer);
            ClassId1 = Short.Create(buffer);
            MethodId1 = Short.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
