using AMQP_0_9_1.Listener;
using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IConnectionListenerFactory
    {
        /// <summary>
        /// Create connection listener
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Connection listener</returns>
        IConnectionListener Create(Address address);
    }
}