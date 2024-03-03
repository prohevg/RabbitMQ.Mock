using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class BasicQos : IIncomingAmqpMethod
    {
        public Long Prefetch­Size { get; set; }
        public Short PrefetchCount { get; set; }
        public Octet Global { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Basic);

        public Short MethodId => Short.Create(BasicMethodConstants.Qos);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += PrefetchSize.SizeOf();
            bufferSize += PrefetchCount.SizeOf();
            bufferSize += Global.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            PrefetchSize = Long.Create(buffer);
            PrefetchCount = Short.Create(buffer);
            Global = Octet.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }

}
