using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IQueueFactory
    {
        /// <summary>
        /// Create queue 
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>New queue</returns>
        IQueue Create(string name);
    }
}