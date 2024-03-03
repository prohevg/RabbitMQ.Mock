using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public class QueueFactory : IQueueFactory
    {
        public IQueue Create(string name)
        {
            return new Queue(name);
        }
    }
}