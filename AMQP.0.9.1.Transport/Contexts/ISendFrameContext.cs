using AMQP_0_9_1.Framing;
using System.Collections.Generic;

namespace AMQP_0_9_1.Transport.Contexts
{
    public interface ISendFrameContext
    {
        IAmqpFrameMethod Method { get; }

        IAmqpFrameContentHeader Header { get; }

        LinkedList<IAmqpFrameContent> Content { get; }
    }
}