using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Exceptions;
using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Methods.Incoming;
using AMQP_0_9_1.Methods.Outgoing;
using AMQP_0_9_1.Transport;
using AMQP_0_9_1.Transport.Contexts;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Factories;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Settings;
using AMQP_0_9_1.Types;
using System;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Domain
{
    /// <summary>
    /// The Connection class represents an AMQP connection.
    /// </summary>
    public class Connection : IConnection
    {
        #region fileds

        private readonly IAsyncTransport _transport;
        private readonly IAmqpMethodFactory _methodFactory;
        private readonly IChannelFactory _channelFactory;
        private readonly ConnectionSettings _settings;
        private readonly AmqpReader _reader;
        private ITransport _writer;
        private readonly IChannelList _channels;

        #endregion

        public Connection(IAsyncTransport transport, 
            IAmqpMethodFactory methodFactory, 
            IChannelFactory channelFactory, 
            IChannelList channels,
            ConnectionSettings settings)
        {
            _transport = transport;
            _methodFactory = methodFactory;
            _channelFactory = channelFactory;
            _channels = channels;
            _settings = settings;

            Id = Guid.NewGuid().ToString();

            _reader = new AmqpReader(_transport, _settings.MaxFrameSize, OnReadIoException);
            _writer = new AmqpWriter(_transport, OnWriteIoException);
        }

        #region IConnection

        public string Id { get; private set; }

        public ValueTask OpenAsync()
        {
            _reader.Start(this);

            SendMethod(0, new ConnectionStart());

            return ValueTask.CompletedTask;
        }

        public async ValueTask CloseAsync()
        {
            foreach (var channel in _channels.List)
            {
                await channel.CloseAsync();

                channel.Dispose();

                _channels.Remove(channel.Id.ToString());
            }
        }

        public bool OnHeader(ProtocolHeader header)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "RECV AMQP {0}", header);
            if (header.Id != 0 || header.Major != 0 || header.Minor != 9 || header.Revision != 1)
            {
                throw new AmqpException("Mismatch 0 0 9 1 header");
            }

            return true;
        }

        public async ValueTask<bool> OnFrameAsync(IReceiveFrameContext context)
        {
            if (context.Method.FrameType == (int)FrameType.FrameHeartbeat)
            {
                SendFrameHeartbeat();
                return true;
            }

            var amqpMethod = _methodFactory.CreateIncomingMethod(context.Method);

            if (amqpMethod is ConnectionStartOk)
            {
                var connTune = new ConnectionTune
                {
                    FrameMax = Long.Create(_settings.MaxFrameSize),
                    ChannelMax = Short.Create((ushort)_settings.MaxChannels),
                    Heartbeat = Short.Create((ushort)_settings.Heartbeat.TotalSeconds)
                };

                SendMethod(0, connTune);
                return true;
            }

            if (amqpMethod is ConnectionTuneOk)
            {
                return true;
            }

            if (amqpMethod is ConnectionOpen)
            {
                SendMethod(0, new ConnectionOpenOk());
                return true;
            }

            if (amqpMethod is ChannelOpen)
            {
                var newChannel = _channelFactory.Create(context.Method.ChannelId, this);
                _channels.Add(newChannel.Id.ToString(), newChannel);
                SendMethod(newChannel.Id, new ChannelOpenOk());

                return true;
            }

            if (_channels.TryGetValue(((uint)context.Method.ChannelId).ToString(), out var channel))
            {
                return await channel.OnFrameAsync(context).ConfigureAwait(false);
            }

            return true;
        }

        public void SendMethod(ushort channelId, IOutgoingAmqpMethod method)
        {
            IAmqpFrameMethod frame = new AmqpFrameMethod
            {
                FrameType = 1,
                ChannelId = Short.Create(channelId), 
            };

            var buffer = frame.WriteTo(method);
            
            _writer.Send(buffer);

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "SEND (ch={0}) {1}", channelId, method.ToString());
            AmqpTrace.WriteBuffer("SEND {0}", buffer.Buffer, buffer.Offset, buffer.Length);

            //if (this.heartBeat != null)
            //{
            //    this.heartBeat.OnSend();
            //}
        }

        public void SendFrames(ushort channelId, ISendFrameContext context)
        {
            _writer.Send(context.Method.WriteTo());

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "SEND (ch={0}) {1}", channelId, context.Method);

            _writer.Send(context.Header.WriteTo());

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "SEND Header (ch={0}) {1}", channelId, context.Header.BodySize);

            foreach (var content in context.Content) 
            {
                _writer.Send(content.WriteTo());

                AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "SEND Content (ch={0}) {1}", channelId, context.Content.Count);
            }
        }

        public event EventHandler<ConnectionCloseEventArgs> Close;

        #endregion

        #region private

        private void SendFrameHeartbeat(ushort channelId = 0)
        {
            return;
            IAmqpFrameMethod frame = new AmqpFrameMethod
            {
                FrameType = Octet.Create((byte)FrameType.FrameHeartbeat),
                ChannelId = Short.Create(channelId)
            };

            var buffer = frame.WriteTo(null);

            _writer.Send(buffer);

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "SEND  (ch={0})", channelId);
            AmqpTrace.WriteBuffer("SEND {0}", buffer.Buffer, buffer.Offset, buffer.Length);
        }

        private void RaiseCloseClient()
        {
            var handler = Close;
            if (handler != null)
            {
                handler.Invoke(this, new ConnectionCloseEventArgs(Id));
            }
        }

        private void OnReadIoException(Exception exception)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Error, "Read I/O: {0}", exception.ToString());

            RaiseCloseClient();
        }

        private void OnWriteIoException(Exception exception)
        {
            AmqpTrace.WriteLine(AmqpTraceLevel.Error, "Write I/O: {0}", exception.ToString());

            RaiseCloseClient();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
        }
       
        #endregion
    }
}
