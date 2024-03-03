using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    /// <summary>
    /// long-long-uint = 8*OCTET
    /// ulong c#
    /// </summary>
    public class LongLong : BaseAmqpType<ulong>, IAmqpType
    {
        #region Construstor

        private LongLong(ulong value)
            : base(value)
        {
        }

        private LongLong(ByteBuffer buffer)
            : base(buffer)
        {
        }

        public static LongLong Create(ulong val)
        {
            return new LongLong(val);
        }

        public static LongLong Create(long val)
        {
            return new LongLong((ulong)val);
        }

        public static LongLong Create(ByteBuffer buffer)
        {
            return new LongLong(buffer);
        }

        public static LongLong Empty => new LongLong(0);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            WriteLong(buffer, _value);
        }

        public int TypeSize => 8;

        #endregion

        #region override

        protected override ulong ReadFromBuffer(ByteBuffer buffer)
        {
            return ReadULong(buffer);
        }

        #endregion

        #region implicit

        public static implicit operator ulong(LongLong value)
        {
            return value._value;
        }

        public static implicit operator long(LongLong value)
        {
            return (int)value._value;
        }

        public static implicit operator LongLong(ulong value)
        {
            return new LongLong(value);
        }

        public static implicit operator LongLong(long value)
        {
            return new LongLong((ulong)value);
        }

        #endregion

        #region private

        /// <summary>
        /// Reads a 64-bit signed integer from the buffer and advance the buffer read cursor.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <returns></returns>
        public ulong ReadULong(ByteBuffer buffer)
        {
            buffer.ValidateRead(TypeSize);
            long high = ReadInt(buffer.Buffer, buffer.Offset);
            long low = (uint)ReadInt(buffer.Buffer, buffer.Offset + 4);
            long data = (high << 32) | low;
            buffer.Complete(TypeSize);
            return (ulong)data;
        }

        private int ReadInt(byte[] buffer, int offset)
        {
            return buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8 | buffer[offset + 3];
        }

        public void WriteLong(ByteBuffer buffer, ulong data)
        {
            Long.Create((uint)(data >> 32)).WriteTo(buffer);
            Long.Create((uint)data).WriteTo(buffer);
        }

        #endregion

        public override string ToString()
        {
            return $"LongLong={_value}";
        }
    }
}
