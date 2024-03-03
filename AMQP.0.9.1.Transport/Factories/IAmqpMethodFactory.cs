using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Methods;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IAmqpMethodFactory
    {
        IIncomingAmqpMethod CreateIncomingMethod(IAmqpFrameMethod frame);
    }
}
