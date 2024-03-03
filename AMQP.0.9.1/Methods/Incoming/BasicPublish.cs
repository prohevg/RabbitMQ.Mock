using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class BasicPublish : IIncomingAmqpMethod
    {
        public Short Reserved1 { get; set; }
        public Shortstr Exchange { get; set; }
        public Shortstr RoutingKey { get; set; }
        public Boolean Mandatory { get; set; }
        public Boolean Immediate { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(60);

        public Short MethodId => Short.Create(40);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            bufferSize += Exchange.SizeOf();
            bufferSize += RoutingKey.SizeOf();
            bufferSize += AmqpBits.Empty.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            Reserved1 = Short.Create(buffer);
            Exchange = Shortstr.Create(buffer);
            RoutingKey = Shortstr.Create(buffer);

            AmqpBits.Create(buffer).ReadValues(out var mandatory, out var immediate);
            Mandatory = Boolean.Create(mandatory);
            Immediate = Boolean.Create(immediate);

            return buffer.Offset - offset;
        }

        #endregion
    }
}
