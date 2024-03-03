using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class QueueDeclare : IIncomingAmqpMethod
    {
        public Short Reserved1 { get; set; }
        public Shortstr Queue { get; set; }
        public Boolean Passive { get; set; }
        public Boolean Durable { get; set; }
        public Boolean Exclusive { get; set; }
        public Boolean AutoDelete { get; set; }
        public Boolean NoWait { get; set; }
        public Table Arguments { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(30);

        public Short MethodId => Short.Create(10);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            bufferSize += Queue.SizeOf();
            bufferSize += AmqpBits.Empty.SizeOf();
            bufferSize += Arguments.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            Reserved1 = Short.Create(buffer);
            Queue = Shortstr.Create(buffer);

            AmqpBits.Create(buffer).ReadValues(out var passive, out var durable, out var exclusive, out var autoDelete, out var noWait);
            Passive = Boolean.Create(passive);
            Durable = Boolean.Create(durable);
            Exclusive = Boolean.Create(exclusive);
            AutoDelete = Boolean.Create(autoDelete);
            NoWait = Boolean.Create(noWait);

            Arguments = Table.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
