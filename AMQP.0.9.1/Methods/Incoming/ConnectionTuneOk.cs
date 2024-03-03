using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ConnectionTuneOk : IIncomingAmqpMethod
    {
        public Short ChannelMax { get; set; }
        public Long FrameMax { get; set; }
        public Short Heartbeat { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(31);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ChannelMax.SizeOf();
            bufferSize += FrameMax.SizeOf();
            bufferSize += Heartbeat.SizeOf();
            return Long.Create(bufferSize);
        }

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            ChannelMax = Short.Create(buffer);
            FrameMax = Long.Create(buffer);
            Heartbeat = Short.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
