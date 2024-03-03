using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    public class AmqpFrameMethod : AmqpFrame, IAmqpFrameMethod
    {
        public const int FrameRequiredSize = 4;

        public AmqpFrameMethod(byte[] header)
            : base(header)
        {            
        }

        public AmqpFrameMethod()
        {
        }

        #region IAmqpFrameMethod

        public bool IsExpectedBody { get; private set; }

        public ByteBuffer Payload { get; set; }

        public override void ReadPayload(byte[] payload)
        {
            var buffer = new ByteBuffer(payload.Length, false);
            buffer.WriteBytes(payload, 0, payload.Length);

            var classId = Short.Create(buffer);
            var methodId = Short.Create(buffer);

            IsExpectedBody = classId == ClassConstants.Basic && methodId == BasicMethodConstants.Publish;

            buffer.Seek(0);

            Payload = buffer;
        }

        public ByteBuffer WriteTo(IOutgoingAmqpMethod outgoingAmqpMethod)
        {
            if (outgoingAmqpMethod == null)
            {
                var buffer1 = new ByteBuffer(8);

                FrameType.WriteTo(buffer1);
                ChannelId.WriteTo(buffer1);
                Long.Create(1).WriteTo(buffer1);

                Octet.Create(AmqpConstants.FrameEnd).WriteTo(buffer1);

                return buffer1;
            }

            var (frame, payloadSize) = GetRequiredBufferSize(outgoingAmqpMethod);
            var buffer = new ByteBuffer(frame, false);

            FrameType.WriteTo(buffer);
            ChannelId.WriteTo(buffer);

            payloadSize.WriteTo(buffer);
            outgoingAmqpMethod.WriteTo(buffer);

            Octet.Create(AmqpConstants.FrameEnd).WriteTo(buffer);

            return buffer;
        }

        public ByteBuffer WriteTo()
        {
            var buffer = new ByteBuffer(Payload.Length + 8, false);

            FrameType.WriteTo(buffer);
            ChannelId.WriteTo(buffer);

            Long.Create(Payload.Length).WriteTo(buffer);

            buffer.WriteBytes(Payload.Buffer, 0, Payload.Length);

            Octet.Create(AmqpConstants.FrameEnd).WriteTo(buffer);

            return buffer;
        }

        #endregion

        #region private



        private (int frame, Long payloadSize) GetRequiredBufferSize(IAmqpMethod amqpMethod)
        {
            int bufferSize = FrameType.SizeOf();
            bufferSize += ChannelId.SizeOf();
            bufferSize += Long.Empty.SizeOf(); //payloadSize
            bufferSize += Octet.Create(AmqpConstants.FrameEnd).SizeOf();

            var payloadSize = amqpMethod.GetPayloadLength();

            return (bufferSize + payloadSize, payloadSize);
        }

        #endregion
    }
}
