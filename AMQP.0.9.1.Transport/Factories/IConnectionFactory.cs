using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create connection 
        /// </summary>
        /// <param name="transport">Transport</param>
        /// <returns>Connection</returns>
        IConnection Create(IAsyncTransport transport);
    }
}