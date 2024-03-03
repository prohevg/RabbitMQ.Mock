using AMQP_0_9_1.Transport.Domain;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IExchangeFactory
    {
        /// <summary>
        /// Create exchange 
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>New exchange</returns>
        IExchange Create(string name);
    }
}