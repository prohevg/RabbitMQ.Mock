using AMQP_0_9_1.Transport.Extensions;
using System.Net.Sockets;

namespace AMQP_0_9_1.Transport.Settings
{
    /// <summary>
    /// Contains the Tcp settings of a connection.
    /// </summary>
    public class TcpSettings
    {
        private const int DefaultBufferSize = 8192;
        private bool _noDelay = true;
        private int? _receiveBufferSize;
        private int? _receiveTimeout;
        private int? _sendBufferSize;
        private int? _sendTimeout;

        /// <summary>
        /// Specifies the Keep-Alive settings of a Tcp socket. 
        /// If not null, Tcp Keep-Alive is enabled.
        /// </summary>
        public TcpKeepAliveSettings KeepAlive
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the LingerOption option of the Tcp socket.
        /// </summary>
        public LingerOption LingerState
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the NoDelay option of the Tcp socket.
        /// </summary>
        public bool NoDelay
        {
            get
            {
                return _noDelay;
            }
            set
            {
                _noDelay = value;
            }
        }

        /// <summary>
        /// Specifies the ReceiveBufferSize option of the Tcp socket.
        /// </summary>
        public int ReceiveBufferSize
        {
            get
            {
                return _receiveBufferSize ?? DefaultBufferSize;
            }
            set
            {
                _receiveBufferSize = value;
            }
        }

        /// <summary>
        /// Specifies the ReceiveTimeout option of the Tcp socket.
        /// </summary>
        public int ReceiveTimeout
        {
            get
            {
                return _receiveTimeout ?? 0;
            }
            set
            {
                _receiveTimeout = value;
            }
        }

        /// <summary>
        /// Specifies the SendBufferSize option of the Tcp socket.
        /// </summary>
        public int SendBufferSize
        {
            get
            {
                return _sendBufferSize ?? DefaultBufferSize;
            }
            set
            {
                _sendBufferSize = value;
            }
        }

        /// <summary>
        /// Specifies the SendTimeout option of the Tcp socket.
        /// </summary>
        public int SendTimeout
        {
            get
            {
                return _sendTimeout ?? 0;
            }
            set
            {
                _sendTimeout = value;
            }
        }

        public void Configure(Socket socket)
        {
            if (KeepAlive != null)
            {
                socket.SetTcpKeepAlive(KeepAlive);
            }

            socket.NoDelay = _noDelay;

            if (_receiveBufferSize != null)
            {
                socket.ReceiveBufferSize = _receiveBufferSize.Value;
            }

            if (_receiveTimeout != null)
            {
                socket.ReceiveTimeout = _receiveTimeout.Value;
            }

            if (_sendBufferSize != null)
            {
                socket.SendBufferSize = _sendBufferSize.Value;
            }

            if (_sendTimeout != null)
            {
                socket.SendTimeout = _sendTimeout.Value;
            }

            if (LingerState != null)
            {
                socket.LingerState = LingerState;
            }
        }
    }
}
