using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Framing;

namespace AMQP_0_9_1.Methods
{
    public interface IOutgoingAmqpMethod : IAmqpMethod
    {
        int WriteTo(ByteBuffer buffer);        
    }
}
