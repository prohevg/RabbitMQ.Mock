using System;

namespace AMQP_0_9_1.Domain
{
    /// <summary>
    /// A byte array wrapper that has a read and a write cursor.
    //
    //   +---------+--------------+----------------+
    // _start      _read          _write             _end
    //
    // _read - _start: already consumed
    // _write - _read: Length (bytes to be consumed)
    // _end - _write: Size (free space to _write)
    // _end - _start: Capacity
    //
    /// </summary>
    public class ByteBuffer
    {
        private byte[] _buffer;
        private int _start;
        private int _read;
        private int _write;
        private int _end;
        private readonly bool _autoGrow;

        /// <summary>
        /// Initializes a new buffer from a byte array.
        /// </summary>
        /// <param name="buffer">The byte array.</param>
        /// <param name="offset">The start position.</param>
        /// <param name="count">The number of bytes.</param>
        /// <param name="capacity">The total size of the byte array from offset.</param>
        public ByteBuffer(byte[] buffer, int offset, int count, int capacity)
            : this(buffer, offset, count, capacity, false)
        {
        }

        /// <summary>
        /// Initializes a new buffer of a specified size.
        /// </summary>
        /// <param name="size">The size in bytes.</param>
        /// <param name="autoGrow">If the buffer should auto-grow when a write size is larger than the buffer size.</param>
        public ByteBuffer(int size, bool autoGrow = true)
            : this(new byte[size], 0, 0, size, autoGrow)
        {
        }

        public ByteBuffer(byte[] buffer, int offset, int count, int capacity, bool autoGrow = true)
        {
            _buffer = buffer;
            _start = offset;
            _read = offset;
            _write = offset + count;
            _end = offset + capacity;
            _autoGrow = autoGrow;
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _end - _start;
            }
        }

        /// <summary>
        /// Gets the current offset (read position).
        /// </summary>
        public int Offset
        {
            get
            {
                return _read;
            }
        }

        /// <summary>
        /// Gets the remaining size for write.
        /// </summary>
        public int Size
        {
            get
            {
                return _end - _write;
            }
        }

        /// <summary>
        /// Gets the available size for read.
        /// </summary>
        public int Length
        {
            get
            {
                return _write - _read;
            }
        }

        /// <summary>
        /// Gets the write position.
        /// </summary>
        public int WritePos
        {
            get
            {
                return _write;
            }
        }

        /// <summary>
        /// Verifies that the buffer has enough bytes for read or enough room for write and grow the buffer if needed.
        /// </summary>
        /// <param name="write">Operation to verify. True for write and false for read.</param>
        /// <param name="dataSize">The size to read or write.</param>
        public void Validate(bool write, int dataSize)
        {
            if (write)
            {
                ValidateWrite(dataSize);
            }
            else
            {
                ValidateRead(dataSize);
            }
        }

        /// <summary>
        /// Verifies that the buffer has enough bytes for read.
        /// </summary>
        /// <param name="dataSize">The size to read.</param>
        public void ValidateRead(int dataSize)
        {
            if (Length < dataSize)
            {
                ThrowBufferTooSmallException();
            }
        }

        /// <summary>
        /// Verifies that the buffer has enough room for write and grow the buffer if needed.
        /// </summary>
        /// <param name="dataSize">The size to write.</param>
        public void ValidateWrite(int dataSize)
        {
            if (Size < dataSize)
            {
                TryAutoGrowBuffer(dataSize);
            }
        }

        private void TryAutoGrowBuffer(int dataSize)
        {
            if (Size < dataSize && _autoGrow)
            {
                int newSize = Math.Max(Capacity * 2, Capacity + dataSize);
                byte[] newBuffer;
                int offset;
                int count;
                DuplicateBuffer(newSize, _write - _start, out newBuffer, out offset, out count);

                int bufferOffset = _start - offset;

                _buffer = newBuffer;
                _start = offset;
                _read -= bufferOffset;
                _write -= bufferOffset;
                _end = offset + count;
            }

            bool valid = Size >= dataSize;

            if (!valid)
            {
                ThrowBufferTooSmallException();
            }
        }

        private static void ThrowBufferTooSmallException()
        {
            throw new InvalidOperationException("buffer too small");
        }

        /// <summary>
        /// Advances the write position. As a result, length is increased by size.
        /// </summary>
        /// <param name="size">Size to advance.</param>
        public void Append(int size)
        {
            _write += size;
        }

        /// <summary>
        /// Advances the read position.
        /// </summary>
        /// <param name="size">Size to advance.</param>
        public void Complete(int size)
        {
            _read += size;
        }

        /// <summary>
        /// Sets the read position.
        /// </summary>
        /// <param name="seekPosition">The position relative to <see cref="Offset"/> of the buffer.</param>
        public void Seek(int seekPosition)
        {
            _read = _start + seekPosition;
        }

        /// <summary>
        /// Moves back the write position. As a result, length is decreased by size.
        /// </summary>
        /// <param name="size"></param>
        public void Shrink(int size)
        {
            _write -= size;
        }

        /// <summary>
        /// Resets read and write position to the initial state.
        /// </summary>
        public void Reset()
        {
            _read = _start;
            _write = _start;
        }

        /// <summary>
        /// Adjusts the read and write position.
        /// </summary>
        /// <param name="offset">Read position to set.</param>
        /// <param name="length">Length from read position to set the write position.</param>
        public void AdjustPosition(int offset, int length)
        {
            _read = offset;
            _write = _read + length;
        }

        public virtual void DuplicateBuffer(int bufferSize, int dataSize, out byte[] buffer, out int offset, out int count)
        {
            buffer = new byte[bufferSize];
            offset = 0;
            count = bufferSize;
            Array.Copy(_buffer, _start, buffer, 0, dataSize);
        }

        public int Start
        {
            get { return _start; }
        }

        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(_buffer, _start, Capacity);
        }

        public virtual void AddReference()
        {
        }

        public virtual void ReleaseReference()
        {
        }
    }
}
