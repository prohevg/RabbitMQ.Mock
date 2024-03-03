using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Extensions
{
    static class SocketExtensions
    {
        public static void SetTcpKeepAlive(this Socket socket, TcpKeepAliveSettings settings)
        {
            int bytesPerUInt = 4;
            byte[] inOptionValues = new byte[bytesPerUInt * 3];

            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes(settings.KeepAliveTime).CopyTo(inOptionValues, bytesPerUInt);
            BitConverter.GetBytes(settings.KeepAliveInterval).CopyTo(inOptionValues, bytesPerUInt * 2);

            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        public static void Complete<T>(object sender, SocketAsyncEventArgs args, bool throwOnError, T result)
        {
            var tcs = (TaskCompletionSource<T>)args.UserToken;
            args.UserToken = null;
            if (tcs == null)
            {
                return;
            }

            if (args.SocketError != SocketError.Success && throwOnError)
            {
                tcs.TrySetException(new SocketException((int)args.SocketError));
            }
            else
            {
                tcs.TrySetResult(result);
            }
        }

        public static Task ConnectAsync(this Socket socket, IPAddress addr, int port)
        {
            var tcs = new TaskCompletionSource<int>();
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(addr, port);
            args.UserToken = tcs;
            args.Completed += (s, a) => { Complete(s, a, true, 0); a.Dispose(); };
            if (!socket.ConnectAsync(args))
            {
                Complete(socket, args, true, 0);
                args.Dispose();
            }

            return tcs.Task;
        }

        public static Task<int> ReceiveAsync(this Socket socket, SocketAsyncEventArgs args, byte[] buffer, int offset, int count)
        {
            var tcs = new TaskCompletionSource<int>();
            args.SetBuffer(buffer, offset, count);
            args.UserToken = tcs;
            if (!socket.ReceiveAsync(args))
            {
                Complete(socket, args, true, args.BytesTransferred);
            }

            return tcs.Task;
        }

        public static Task<int> SendAsync(this Socket socket, SocketAsyncEventArgs args, IList<ArraySegment<byte>> buffers)
        {
            var tcs = new TaskCompletionSource<int>();
            args.SetBuffer(null, 0, 0);
            args.BufferList = buffers;
            args.UserToken = tcs;
            if (!socket.SendAsync(args))
            {
                Complete(socket, args, true, 0);
            }

            return tcs.Task;
        }

        public static Task<Socket> AcceptAsync(this Socket socket, SocketAsyncEventArgs args, SocketFlags flags)
        {
            var tcs = new TaskCompletionSource<Socket>();
            args.UserToken = tcs;
            if (!socket.AcceptAsync(args))
            {
                Complete(socket, args, false, args.AcceptSocket);
            }

            return tcs.Task;
        }
    }
}
