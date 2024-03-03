using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Transport.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport
{
    public class TcpTransport : IAsyncTransport
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _sendArgs;
        private readonly SocketAsyncEventArgs _receiveArgs;
        private ByteBuffer _receiveBuffer;

        public TcpTransport(Socket socket)
        {
            _socket = socket;

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (s, a) => SocketExtensions.Complete(s, a, true, 0);

            _receiveArgs = new SocketAsyncEventArgs();
            _receiveArgs.Completed += (s, a) => SocketExtensions.Complete(s, a, true, a.BytesTransferred);
        }

        #region IAsyncTransport

        public Task SendAsync(IList<ByteBuffer> bufferList, int listSize)
        {
            var segments = new ArraySegment<byte>[bufferList.Count];
            for (int i = 0; i < bufferList.Count; i++)
            {
                ByteBuffer f = bufferList[i];
                segments[i] = new ArraySegment<byte>(f.Buffer, f.Offset, f.Length);
            }

            return _socket.SendAsync(_sendArgs, segments);
        }

        public async Task<int> ReceiveAsync(byte[] buffer, int offset, int count)
        {
            if (_receiveBuffer != null && _receiveBuffer.Length > 0)
            {
                try
                {
                    _receiveBuffer.AddReference();
                    return ReceiveFromBuffer(_receiveBuffer, buffer, offset, count);
                }
                finally
                {
                    _receiveBuffer.ReleaseReference();
                }
            }

            if (_receiveBuffer == null)
            {
                return await _socket.ReceiveAsync(_receiveArgs, buffer, offset, count).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    _receiveBuffer.AddReference();

                    int bytes = await _socket.ReceiveAsync(_receiveArgs, _receiveBuffer.Buffer, _receiveBuffer.WritePos, _receiveBuffer.Size).ConfigureAwait(false);

                    _receiveBuffer.Append(bytes);

                    return ReceiveFromBuffer(_receiveBuffer, buffer, offset, count);
                }
                finally
                {
                    _receiveBuffer.ReleaseReference();
                }
            }
        }

        #endregion

        #region ITransport

        public void Send(ByteBuffer buffer)
        {
            _socket.Send(buffer.Buffer, buffer.Offset, buffer.Length, SocketFlags.None);
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            return _socket.Receive(buffer, offset, count, SocketFlags.None);
        }

        public void Close()
        {
            _sendArgs.Dispose();
            _receiveArgs.Dispose();
            _socket.Dispose();

            var temp = _receiveBuffer;
            if (temp != null)
            {
                temp.ReleaseReference();
            }
        }

        #endregion

        #region private

        private static int ReceiveFromBuffer(ByteBuffer byteBuffer, byte[] buffer, int offset, int count)
        {
            int len = byteBuffer.Length;
            if (len <= count)
            {
                Buffer.BlockCopy(byteBuffer.Buffer, byteBuffer.Offset, buffer, offset, len);
                byteBuffer.Reset();
                return len;
            }
            else
            {
                Buffer.BlockCopy(byteBuffer.Buffer, byteBuffer.Offset, buffer, offset, count);
                byteBuffer.Complete(count);
                return count;
            }
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}