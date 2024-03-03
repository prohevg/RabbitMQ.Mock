using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Workers
{
    public interface IConsumerPublisher
    {
        /// <summary>
        /// Publish message
        /// </summary>
        Task PublishAsync(CancellationToken token);
    }
}
