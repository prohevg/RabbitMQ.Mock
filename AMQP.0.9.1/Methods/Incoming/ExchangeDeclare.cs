using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using System.Collections.Generic;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ExchangeDeclare : IIncomingAmqpMethod
    {
        public Short Reserved1 { get; set; }
        public Shortstr Exchange { get; set; }
        public Shortstr Type { get; set; }
        public Boolean Passive { get; set; }
        public Boolean Durable { get; set; }
        public Boolean AutoDelete { get; set; }
        public Boolean Internal { get; set; }
        public Boolean NoWait { get; set; }
        public Table Arguments { get; set; }


        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Exchange);

        public Short MethodId => Short.Create(ExchangeMethodConstants.Declare);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Reserved1.SizeOf();
            bufferSize += Exchange.SizeOf();
            bufferSize += Type.SizeOf();
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
            Exchange = Shortstr.Create(buffer);
            Type = Shortstr.Create(buffer);

            AmqpBits.Create(buffer).ReadValues(out var passive, out var durable, out var intern, out var autoDelete, out var noWait);
            Passive = Boolean.Create(passive);
            Durable = Boolean.Create(durable);
            Internal = Boolean.Create(intern);
            AutoDelete = Boolean.Create(autoDelete);
            NoWait = Boolean.Create(noWait);

            Arguments = Table.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
