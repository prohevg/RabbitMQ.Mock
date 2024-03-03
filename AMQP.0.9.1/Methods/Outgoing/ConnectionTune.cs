using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class ConnectionTune : IOutgoingAmqpMethod
    {
        public Short ChannelMax { get; set; }
        public Long FrameMax { get; set; }
        public Short Heartbeat { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(30);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ChannelMax.SizeOf();
            bufferSize += FrameMax.SizeOf();
            bufferSize += Heartbeat.SizeOf();
            return Long.Create(bufferSize);
        }

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            ChannelMax.WriteTo(buffer);
            FrameMax.WriteTo(buffer);
            Heartbeat.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
