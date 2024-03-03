using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Transport.Contexts;
using System;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Id connection
        /// </summary>
        string Id { get; }

        /// <summary>
        /// OpenAsync connection
        /// </summary>
        ValueTask OpenAsync();

        /// <summary>
        /// CloseAsync connection
        /// </summary>
        ValueTask CloseAsync();

        /// <summary>
        /// Receive header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        bool OnHeader(ProtocolHeader header);

        /// <summary>
        /// Receive frame
        /// </summary>
        /// <param name="context">Frame context</param>
        ValueTask<bool> OnFrameAsync(IReceiveFrameContext context);

        /// <summary>
        /// Send method
        /// </summary>
        void SendMethod(ushort channelId, IOutgoingAmqpMethod method);

        /// <summary>
        /// Send frames
        /// </summary>
        void SendFrames(ushort channelId, ISendFrameContext context);

        /// <summary>
        /// Close client event
        /// </summary>
        event EventHandler<ConnectionCloseEventArgs> Close;
    }

    public class ConnectionCloseEventArgs : EventArgs
    {
        public ConnectionCloseEventArgs(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}
