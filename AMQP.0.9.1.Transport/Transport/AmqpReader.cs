using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Exceptions;
using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Transport.Contexts;
using AMQP_0_9_1.Transport.Domain;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport
{
    public class AmqpReader : IDisposable
    {
        private readonly IAsyncTransport _transport;
        private readonly int _maxFrameSize;
        private readonly Action<Exception> _onException;

        public AmqpReader(IAsyncTransport transport, int maxFrameSize, Action<Exception> onException)
        {
            _transport = transport;
            _maxFrameSize = maxFrameSize;
            _onException = onException;
        }

        public void Start(IConnection connection)
        {
            _ = StartAsync(connection);
        }

        public async Task ReadAsync(IConnection connection, int maxFrameSize)
        {
            var header = new byte[8];
            
            // header
            await this.ReceiveBufferAsync(header, 0, header.Length).ConfigureAwait(false);

            AmqpTrace.WriteBuffer("RECV HEADER {0}", header, 0, header.Length);

            if (connection.OnHeader(ProtocolHeader.Create(header, 0)) == false)
            {
                return;
            }

            // frames
            while (true)
            {        
                var method = await ReadFrameMethodAsync().ConfigureAwait(false);
                if (method.IsExpectedBody == false)
                {
                    await connection.OnFrameAsync(new ReceiveFrameContext(method));
                    continue;
                }

                var contentHeader = await ReadFrameContentHeaderAsync().ConfigureAwait(false);

                var remainder = contentHeader.BodySize;
                var contents = new LinkedList<IAmqpFrameContent>();
                while (remainder > 0)
                {
                    var content = await ReadFrameContentAsync(maxFrameSize).ConfigureAwait(false);

                    remainder -= (uint)content.PayloadLength;

                    contents.AddLast(content);
                }

                await connection.OnFrameAsync(new ReceiveFrameContext(method, contentHeader, contents));
            }
        }

        #region private

        private async Task<IAmqpFrameMethod> ReadFrameMethodAsync()
        {
            var frameHeader = new byte[AmqpFrame.HeaderSize];

            await this.ReceiveBufferAsync(frameHeader, 0, frameHeader.Length).ConfigureAwait(false);

            IAmqpFrameMethod frame = new AmqpFrameMethod(frameHeader);

            var payLoad = new byte[frame.PayloadLength + 1]; //+1 end frame

            await this.ReceiveBufferAsync(payLoad, 0, payLoad.Length).ConfigureAwait(false);

            if (payLoad[payLoad.Length - 1] != AmqpConstants.FrameEnd)
            {
                AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "Incorrect method frame");
                throw new AmqpReaderException("Incorrect methos frame");
            }

            payLoad[payLoad.Length - 1] = 0; //remove end frame

            if (frame.FrameType == (int)FrameType.FrameHeartbeat)
            {
                return frame;
            }

            frame.ReadPayload(payLoad);

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "RECV Method Frame ChannelId={0}, PayloadSize={1}", frame.ChannelId, frame.PayloadLength);

            return frame;
        }

        private async Task<IAmqpFrameContentHeader> ReadFrameContentHeaderAsync()
        {
            var frameHeader = new byte[AmqpFrame.HeaderSize + AmqpFrameContentHeader.FrameRequiredSize];

            await this.ReceiveBufferAsync(frameHeader, 0, frameHeader.Length).ConfigureAwait(false);

            IAmqpFrameContentHeader frame = new AmqpFrameContentHeader(frameHeader);

            var payload = new byte[frame.PayloadLength - AmqpFrameContentHeader.FrameRequiredSize + 1]; //+1 end frame

            await this.ReceiveBufferAsync(payload, 0, payload.Length).ConfigureAwait(false);

            if (payload[payload.Length - 1] != AmqpConstants.FrameEnd)
            {
                AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "Incorrect header content frame");
                throw new AmqpReaderException("Incorrect header content frame");
            }

            payload[payload.Length - 1] = 0; //remove end frame

            frame.ReadPayload(payload);

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "RECV Contect Header Frame ChannelId={0}, ClassId={1}, PayloadLength={2}", frame.ChannelId, frame.ClassId, frame.PayloadLength);

            return frame;
        }

        private async Task<IAmqpFrameContent> ReadFrameContentAsync(int maxFrameSize)
        {
            var frameHeader = new byte[AmqpFrame.HeaderSize];

            await this.ReceiveBufferAsync(frameHeader, 0, frameHeader.Length).ConfigureAwait(false);

            IAmqpFrameContent frame = new AmqpFrameContent(frameHeader);

            var payLoad = new byte[frame.PayloadLength + 1]; //+1 end frame

            await this.ReceiveBufferAsync(payLoad, 0, payLoad.Length).ConfigureAwait(false);

            if (payLoad[payLoad.Length - 1] != AmqpConstants.FrameEnd)
            {
                AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "Incorrect content frame");
                throw new AmqpReaderException("Incorrect content frame");
            }

            payLoad[payLoad.Length - 1] = 0; //remove end frame

            frame.ReadPayload(payLoad);

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, "RECV Method Frame ChannelId={0}, PayloadSize={1}", frame.ChannelId, frame.PayloadLength);

            return frame;
        }

        private async Task StartAsync(IConnection connection)
        {
            try
            {
                await this.ReadAsync(connection, _maxFrameSize).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
        }

        private async Task ReceiveBufferAsync(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int received = await _transport.ReceiveAsync(buffer, offset, count).ConfigureAwait(false);
                if (received == 0)
                {
                    throw new OperationCanceledException(_transport.GetType().Name);
                }

                offset += received;
                count -= received;
            }
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            
        }

        #endregion
    }
}
