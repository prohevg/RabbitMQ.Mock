using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IChannelFactory
    {
        /// <summary>
        /// Create channel 
        /// </summary>
        /// <param name="number">Channel number</param>
        /// <param name="connection">Connection</param>
        /// <returns>New channel</returns>
        IChannel Create(int number, IConnection connection);
    }
}