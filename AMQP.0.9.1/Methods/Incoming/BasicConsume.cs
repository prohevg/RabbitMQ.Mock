using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class BasicConsume : IIncomingAmqpMethod
    {
        public Short Reserved1 { get; set; }
        public Shortstr Queue { get; set; }
        public Shortstr ConsumerTag { get; set; }
        public Boolean NoLocal { get; set; }
        public Boolean NoAck { get; set; }
        public Boolean Exclusive { get; set; }
        public Boolean NoWait { get; set; }
        public Table Arguments { get; set; }

        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Basic);

        public Short MethodId => Short.Create(BasicMethodConstants.Consume);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            bufferSize += Queue.SizeOf();
            bufferSize += ConsumerTag.SizeOf();
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
            ConsumerTag = Shortstr.Create(buffer);

            AmqpBits.Create(buffer).ReadValues(out var noLocal, out var noAsc, out var exclusive, out var noWait);
            NoLocal = Boolean.Create(noLocal);
            NoAck = Boolean.Create(noAsc);
            Exclusive = Boolean.Create(exclusive);
            NoWait = Boolean.Create(noWait);

            Arguments = Table.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }

}
