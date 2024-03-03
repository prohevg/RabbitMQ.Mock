using AMQP_0_9_1.Transport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Listener
{
    public interface ITransportListener
    {
        /// <summary>
        /// Open listener
        /// </summary>
        /// <param name="token">Cancellation token</param>
        ValueTask OpenAsync(CancellationToken token);

        /// <summary>
        /// Close listeners
        /// </summary>
        /// <param name="token">Cancellation token</param>
        ValueTask CloseAsync();

        /// <summary>
        /// Accept client event
        /// </summary>
        event EventHandler<AcceptClientEventArgs> AcceptClient;
    }

    public class AcceptClientEventArgs : EventArgs
    {
        public AcceptClientEventArgs(IAsyncTransport asyncTransport)
        {
            AsyncTransport = asyncTransport;
        }

        public IAsyncTransport AsyncTransport { get; }
    }
}