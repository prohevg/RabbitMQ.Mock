using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using Boolean = AMQP_0_9_1.Types.Boolean;

namespace AMQP_0_9_1.Methods.Incoming
{
    public class ExchangeBind : IIncomingAmqpMethod
    {
        public Shortstr Destination { get; set; }

        public Shortstr Source { get; set; }

        public Shortstr RoutingKey { get; set; }

        public Boolean NoWait { get; set; }

        public Table Arguments { get; set; } = new Table();


        #region IAmqpMethod

        public Short ClassId => Short.Create(ClassConstants.Exchange);

        public Short MethodId => Short.Create(ExchangeMethodConstants.Bind);

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += Destination.SizeOf();
            bufferSize += Source.SizeOf();
            bufferSize += RoutingKey.SizeOf();
            bufferSize += Arguments.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion

        #region IIncomingAmqpMethod

        public int ReadTo(ByteBuffer buffer)
        {
            var offset = buffer.Offset;
            Destination = Shortstr.Create(buffer);
            Source = Shortstr.Create(buffer);
            RoutingKey = Shortstr.Create(buffer);
            NoWait = Boolean.Create(buffer);

            //Arguments = Table.Create(buffer);
            return buffer.Offset - offset;
        }

        #endregion
    }
}
