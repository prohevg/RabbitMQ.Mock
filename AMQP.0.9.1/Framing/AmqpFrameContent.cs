using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    public class AmqpFrameContent : AmqpFrame, IAmqpFrameContent
    {
        public AmqpFrameContent()
        {

        }

        public AmqpFrameContent(byte[] header)
            : base(header)
        {
        }

        #region IAmqpFrameContent

        public ByteBuffer Payload { get; set; }

        public override void ReadPayload(byte[] payload)
        {
            var buffer = new ByteBuffer(payload.Length, false);
            buffer.WriteBytes(payload, 0, payload.Length);
            
            Payload = buffer;
        }

        public LongLong GetSize()
        {
            var bufferSize = FrameType.SizeOf();
            bufferSize += ChannelId.SizeOf();
            bufferSize += PayloadLength.SizeOf();
            bufferSize += Payload.Length;
            bufferSize += Octet.Create(AmqpConstants.FrameEnd).SizeOf();
            return LongLong.Create(bufferSize);
        }

        public ByteBuffer WriteTo()
        {
            var buffer = new ByteBuffer(8 + PayloadLength, false);

            FrameType.WriteTo(buffer);
            ChannelId.WriteTo(buffer);
            PayloadLength.WriteTo(buffer);

            buffer.WriteBytes(Payload.Buffer, 0, Payload.Length);

            Octet.Create(AmqpConstants.FrameEnd).WriteTo(buffer);

            return buffer;
        }

        #endregion

        #region override 

        protected override ByteBuffer ParseHeader(byte[] header)
        {
            var buffer = base.ParseHeader(header);

            //BodySize = LongUint.Create(buffer);

            return buffer;
        }

        #endregion
    }
}