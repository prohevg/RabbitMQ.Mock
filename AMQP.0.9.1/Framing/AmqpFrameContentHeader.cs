using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    public class AmqpFrameContentHeader : AmqpFrame, IAmqpFrameContentHeader
    {
        public const int FrameRequiredSize = 12;

        public AmqpFrameContentHeader()
        {

        }

        public AmqpFrameContentHeader(byte[] header)
            : base(header)
        {
            
        }

        #region IAmqpFrameContentHeader

        public Short ClassId { get; set; }

        public Short Weight { get; set; }

        public LongLong BodySize { get; set; }

        public IBasicProperties BasicProperties { get; set; } = new BasicProperties();
       
        public override void ReadPayload(byte[] payload)
        {
            var buffer = new ByteBuffer(payload.Length, false);
            buffer.WriteBytes(payload, 0, payload.Length);

            BasicProperties = new BasicProperties(buffer);
        }

        public ByteBuffer WriteTo()
        {
            var buffer = new ByteBuffer(0);

            FrameType.WriteTo(buffer);
            ChannelId.WriteTo(buffer);

            //header length + empty basic properties (arguments)
            var size = FrameRequiredSize + BasicProperties.GetSize();
            Long.Create(size).WriteTo(buffer);

            ClassId.WriteTo(buffer);
            Weight.WriteTo(buffer);
            BodySize.WriteTo(buffer);

            BasicProperties.WriteTo(buffer);

            Octet.Create(AmqpConstants.FrameEnd).WriteTo(buffer);

            return buffer;
        }

        #endregion

        #region override 

        protected override ByteBuffer ParseHeader(byte[] header)
        {
            var buffer = base.ParseHeader(header);

            ClassId = Short.Create(buffer);
            Weight = Short.Create(buffer);
            BodySize = LongLong.Create(buffer);
            //PropetyFlags = ShortUint.Create(buffer);

            return buffer;
        }

        #endregion
    }
}