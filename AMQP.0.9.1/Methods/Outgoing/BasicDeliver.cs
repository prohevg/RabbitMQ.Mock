using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class BasicDeliver : IOutgoingAmqpMethod
    {
        public Shortstr ConsumerTag { get; set; }
        public LongLong DeliveryTag { get; set; }
        public Boolean Redelivered { get; set; }
        public Shortstr Exchange { get; set; }
        public Shortstr RoutingKey { get; set; }

        public BasicDeliver()
        {
            
        }

        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Basic);

        public Short MethodId => Short.Create(BasicMethodConstants.Deliver);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += ConsumerTag.SizeOf();
            bufferSize += DeliveryTag.SizeOf();
            bufferSize += AmqpBits.Empty.SizeOf();
            bufferSize += Exchange.SizeOf();
            bufferSize += RoutingKey.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IOutgoingAmqpMethod

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            ConsumerTag.WriteTo(buffer);
            DeliveryTag.WriteTo(buffer);

            AmqpBits.Create(new bool[1] { Redelivered }).WriteTo(buffer);

            Exchange.WriteTo(buffer);
            RoutingKey.WriteTo(buffer);
            return buffer.Length - length;
        }

        #endregion
    }
}
