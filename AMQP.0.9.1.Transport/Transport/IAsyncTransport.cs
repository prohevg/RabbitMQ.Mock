using AMQP_0_9_1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport
{
    /// <summary>
    /// Provides asynchronous I/O calls.
    /// </summary>
    public interface IAsyncTransport : ITransport
    {
        /// <summary>
        /// Sends a list of buffers asynchronously.
        /// </summary>
        /// <param name="bufferList">The list of buffers to send.</param>
        /// <param name="listSize">Number of bytes of all buffers in bufferList.</param>
        /// <returns>A task for the send operation.</returns>
        Task SendAsync(IList<ByteBuffer> bufferList, int listSize);

        /// <summary>
        /// Reads bytes into a _buffer asynchronously.
        /// </summary>
        /// <param name="buffer">The _buffer to store data.</param>
        /// <param name="offset">The _buffer offset where data starts.</param>
        /// <param name="count">The number of bytes to _read.</param>
        /// <returns>A task for the receive operation. The result is the actual bytes
        /// _read from the _transport.</returns>
        Task<int> ReceiveAsync(byte[] buffer, int offset, int count);
    }
}
