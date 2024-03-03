using AMQP_0_9_1.Framing;
using System.Collections.Generic;

namespace AMQP_0_9_1.Transport.Contexts
{
    public class ReceiveFrameContext : IReceiveFrameContext
    {
        public ReceiveFrameContext(IAmqpFrameMethod frame, IAmqpFrameContentHeader header = null, LinkedList<IAmqpFrameContent> content = null)
        {
            Method = frame;
            Header = header;
            Content = content;
        }

        public IAmqpFrameMethod Method { get; }

        public IAmqpFrameContentHeader Header { get; }

        public LinkedList<IAmqpFrameContent> Content { get; }
    }
}
