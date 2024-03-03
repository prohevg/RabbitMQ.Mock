using AMQP_0_9_1.Transport.Contexts;
using System;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    public interface IChannel : IDisposable
    {
        /// <summary>
        /// Channel id
        /// </summary>
        ushort Id { get; }

        /// <summary>
        /// OpenAsync
        /// </summary>
        ValueTask OpenAsync();

        /// <summary>
        /// CloseAsync
        /// </summary>
        ValueTask CloseAsync();

        /// <summary>
        /// Receive frame
        /// </summary>
        ValueTask<bool> OnFrameAsync(IReceiveFrameContext context);
    }
}
