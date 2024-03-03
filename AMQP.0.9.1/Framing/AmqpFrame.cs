using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Exceptions;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    public abstract class AmqpFrame : IAmqpFrame
    {
        public const int HeaderSize = 7;

        protected AmqpFrame() 
        { 
        }

        protected AmqpFrame(byte[] header)
        {
            //if (header == null || header.Length != HeaderSize)
            //{
            //    throw new AmqpFrameException();
            //}

            ParseHeader(header);
        }

        #region IAmqpFrame

        public Octet FrameType { get; set; }

        public Short ChannelId { get; set; }

        public Long PayloadLength { get; set; }

        public abstract void ReadPayload(byte[] payload);

        #endregion

        #region private

        protected virtual ByteBuffer ParseHeader(byte[] header)
        {
            var buffer = new ByteBuffer(header.Length, false);
            buffer.WriteBytes(header, 0, header.Length);

            FrameType = Octet.Create(buffer);
            ChannelId = Short.Create(buffer);
            PayloadLength = Long.Create(buffer);

            return buffer;
        }

        #endregion
    }
}