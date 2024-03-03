using AMQP_0_9_1.Transport;
using AMQP_0_9_1.Transport.Factories;
using AMQP_0_9_1.Transport.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Listener
{
    public class TcpTransportListener : ITransportListener
    {
        #region fields

        private List<TcpListener> _listeners;
        private bool _closed;
        private readonly IAddressFactory _addressFactory;
        private readonly string _host;
        private readonly int _port;
        private readonly TcpSettings _tcpSettings;

        #endregion

        public TcpTransportListener(IAddressFactory addressFactory, string host, int port, TcpSettings tcpSettings)
        {
            _addressFactory = addressFactory;
            _host = host;
            _port = port;
            _tcpSettings = tcpSettings;

            _listeners = new List<TcpListener>();
        }

        #region ITransportListener

        public ValueTask OpenAsync(CancellationToken token)
        {
            var addresses = _addressFactory.Create(_host);

            _listeners = new List<TcpListener>(addresses.Count);

            foreach (var address in addresses)
            {
                var listener = CreateTcpListener(address, _port);

                listener.Start();

                _ = AcceptTcpClientAsync(listener, token).ConfigureAwait(false);

                _listeners.Add(listener);
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask CloseAsync()
        {
            _closed = true;

            if (_listeners == null)
            {
                return ValueTask.CompletedTask;
            }

            foreach (var listen in _listeners)
            {
                listen.Stop();
                listen.Server.Dispose();
            }

            return ValueTask.CompletedTask;
        }

        public event EventHandler<AcceptClientEventArgs> AcceptClient;

        #endregion

        #region private

        private TcpListener CreateTcpListener(IPAddress address, int port)
        {
            var listener = new TcpListener(address, port);            

            _tcpSettings.Configure(listener.Server);

            return listener;
        }

        private async ValueTask AcceptTcpClientAsync(TcpListener listener, CancellationToken token)
        {

            try
            {
                while (!_closed)
                {
                    try
                    {
                        var acceptSocket = await listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
                        if (acceptSocket != null)
                        {
                            _ = HandleSocketAsync(acceptSocket.Client).ConfigureAwait(false);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (Exception ex)
                    {
                        AmqpTrace.WriteLine(AmqpTraceLevel.Warning, ex.ToString());
                    }
                }

                listener.Stop();
            }
            finally
            {
                _listeners.Remove(listener);                
            }
        }

        private async ValueTask HandleSocketAsync(Socket socket)
        {
            try
            {
                var asyncTransport = await CreateAsyncTransportAsync(socket).ConfigureAwait(false);

                RaiseAcceptClient(asyncTransport);
            }
            catch (Exception ex)
            {
                AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.ToString());
            }
        }

        private Task<IAsyncTransport> CreateAsyncTransportAsync(Socket socket)
        {
            var tcs = new TaskCompletionSource<IAsyncTransport>();
            tcs.SetResult(new TcpTransport(socket));
            return tcs.Task;
        }

        private void RaiseAcceptClient(IAsyncTransport transport)
        {
            var handler = AcceptClient;
            if (handler != null)
            {
                handler.Invoke(this, new AcceptClientEventArgs(transport));
            }
        }

        #endregion
    }
}
