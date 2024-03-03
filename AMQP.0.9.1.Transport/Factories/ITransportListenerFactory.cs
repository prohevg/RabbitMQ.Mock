using AMQP_0_9_1.Listener;
using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface ITransportListenerFactory
    {
        /// <summary>
        /// Create transport
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns></returns>
        ITransportListener Create(Address address);
    }
}