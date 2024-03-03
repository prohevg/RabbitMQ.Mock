using AMQP_0_9_1.Domain;
using System;

namespace AMQP_0_9_1.Extensions
{
    public static class ByteBufferExtension
    {
        /// <summary>
        /// Reads bytes from one buffer into another.
        /// </summary>
        /// <param name="buffer">Source buffer.</param>
        /// <param name="data">Destination buffer</param>
        /// <param name="offset">The start position to write.</param>
        /// <param name="count">The number of bytes to read.</param>
        public static void ReadBytes(this ByteBuffer buffer, byte[] data, int offset, int count)
        {
            buffer.ValidateRead(count);
            Array.Copy(buffer.Buffer, buffer.Offset, data, offset, count);
            buffer.Complete(count);
        }


        /// <summary>
        /// Writes bytes from one buffer into another.
        /// </summary>
        /// <param name="buffer">The destination buffer.</param>
        /// <param name="data">The source buffer</param>
        /// <param name="offset">The position in source buffer to start.</param>
        /// <param name="count">The number of bytes to write.</param>
        public static void WriteBytes(this ByteBuffer buffer, byte[] data, int offset, int count)
        {
            buffer.ValidateWrite(count);
            Array.Copy(data, offset, buffer.Buffer, buffer.WritePos, count);
            buffer.Append(count);
        }
    }
}