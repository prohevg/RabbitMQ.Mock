using AMQP_0_9_1.Domain;
using System;

namespace AMQP_0_9_1.Transport
{
    /// <summary>
    /// The transport interface used by a connection for network I/O.
    /// </summary>
    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Sends a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to send.</param>
        void Send(ByteBuffer buffer);

        /// <summary>
        /// Receives a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to store the received bytes.</param>
        /// <param name="offset">The start position.</param>
        /// <param name="count">The number of bytes to receive.</param>
        /// <returns>The number of bytes received. It may be less than <paramref name="count"/>.
        /// A value of 0 means that the transport is closed.</returns>
        int Receive(byte[] buffer, int offset, int count);

        /// <summary>
        /// Closes the transport.
        /// </summary>
        void Close();
    }
}