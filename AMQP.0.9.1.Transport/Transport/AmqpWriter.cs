using AMQP_0_9_1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport
{
    public class AmqpWriter : ITransport
    {
        private readonly IAsyncTransport _transport;
        private readonly Action<Exception> _onException;
        private readonly Queue<ByteBuffer> _bufferQueue;
        private bool _writing;
        private bool _closed;

        public AmqpWriter(IAsyncTransport transport, Action<Exception> onException)
        {
            this._transport = transport;
            this._onException = onException;
            this._bufferQueue = new Queue<ByteBuffer>();
        }

        private object SyncRoot
        {
            get 
            { 
                return this._bufferQueue;
            }
        }

        #region ITransport

        public void Send(ByteBuffer buffer)
        {
            lock (this.SyncRoot)
            {
                if (this._closed)
                {
                    buffer.ReleaseReference();
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                this._bufferQueue.Enqueue(buffer);
                if (this._writing)
                {
                    return;
                }

                this._writing = true;
            }

            // Kick off the _write loop
            var _ = this.WriteAsync();
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            lock (this.SyncRoot)
            {
                if (!this._closed)
                {
                    this._closed = true;
                    if (this._writing)
                    {
                        this._bufferQueue.Enqueue(new CloseByteBuffer(this));
                    }
                    else
                    {
                        this._transport.Close();
                    }
                }
            }
        }

        #endregion

        private async Task WriteAsync()
        {
            const int maxBatchSize = 128 * 1024;

            List<ByteBuffer> buffers = new List<ByteBuffer>();
            while (true)
            {
                ByteBuffer buffer = null;
                int size = 0;

                lock (this.SyncRoot)
                {
                    while (size < maxBatchSize && this._bufferQueue.Count > 0)
                    {
                        ByteBuffer item = this._bufferQueue.Dequeue();
                        if (item.Length == 0)   // special _buffer
                        {
                            buffer = item;
                            break;
                        }
                        else
                        {
                            buffers.Add(item);
                            size += item.Length;
                        }
                    }

                    if (size == 0)
                    {
                        this._writing = false;
                        if (buffer == null)
                        {
                            break;
                        }
                    }
                }

                try
                {
                    if (size > 0)
                    {
                        await this._transport.SendAsync(buffers, size).ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    lock (this.SyncRoot)
                    {
                        this._closed = true;
                        this._writing = false;
                        this._transport.Close();
                        buffers.AddRange(this._bufferQueue);
                        this._bufferQueue.Clear();
                    }

                    this._onException(exception);

                    break;
                }
                finally
                {
                    for (int i = 0; i < buffers.Count; i++)
                    {
                        buffers[i].ReleaseReference();
                    }

                    buffers.Clear();
                    if (buffer != null)
                    {
                        buffer.ReleaseReference();
                    }
                }
            }
        }

        #region IDisposable

        public void Dispose()
        {
            _closed = true;
        }

        #endregion

        private class FlushByteBuffer : ByteBuffer
        {
            readonly TaskCompletionSource<object> tcs;

            public FlushByteBuffer()
                : base(null, 0, 0, 0)
            {
                this.tcs = new TaskCompletionSource<object>();
            }

            public Task Task
            {
                get { return this.tcs.Task; }
            }

            public override void ReleaseReference()
            {
                this.tcs.TrySetResult(null);
            }
        }

        private class CloseByteBuffer : ByteBuffer
        {
            readonly AmqpWriter writer;

            public CloseByteBuffer(AmqpWriter writer)
                : base(null, 0, 0, 0)
            {
                this.writer = writer;
            }

            public override void ReleaseReference()
            {
                this.writer._transport.Close();
            }
        }
    }
}
