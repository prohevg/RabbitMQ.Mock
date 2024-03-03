using AMQP_0_9_1.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    public interface IQueue
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Add consumer
        /// </summary>
        /// <param name="consumerTag">Consumer Tag</param>
        void AddConsumer(string consumerTag, InnerConsumer innerConsumer);

        /// <summary>
        /// Remove consumer
        /// </summary>
        /// <param name="id">Channel</param>
        /// <param name="consumerTag">Consumer Tag</param>
        void RemoveConsumer(ushort channelId, string consumerTag);

        /// <summary>
        /// Handle queue
        /// </summary>
        Task HandleAsync(CancellationToken token);

        /// <summary>
        /// Add message
        /// </summary>
        /// <param name="exchange">Source exchange</param>
        /// <param name="message">Message</param>
        void RouteTo(string exchange, byte[] message);
    }
}
