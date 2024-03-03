using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Listener
{
    public interface IConnectionListener
    {
        /// <summary>
        /// Opens the listener.
        /// </summary>
        /// <param name="address">Listen address</param>
        /// <param name="token">Cancellation token</param>
        Task OpenAsync(CancellationToken token);

        /// <summary>
        /// Close listener
        /// </summary>
        Task CloseAsync();

    }
}