using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    /// <summary>
    /// long-uint = 4*OCTET
    /// uint c#
    /// </summary>
    public class Long : BaseAmqpType<uint>, IAmqpType
    {
        #region Construstor

        private Long(uint value)
            : base(value)
        {
        }

        private Long(ByteBuffer buffer)
            : base(buffer)
        {
        }

        public static Long Create(uint val)
        {
            return new Long(val);
        }

        public static Long Create(int val)
        {
            return new Long((uint)val);
        }

        public static Long Create(ByteBuffer buffer)
        {
            return new Long(buffer);
        }

        public static Long Empty => new Long(0);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            WriteInt(buffer, _value);
        }

        public int TypeSize => 4;

        #endregion

        #region override

        protected override uint ReadFromBuffer(ByteBuffer buffer)
        {
            return ReadUInt(buffer);
        }

        #endregion

        #region implicit

        public static implicit operator uint(Long value)
        {
            return value._value;
        }

        public static implicit operator int(Long value)
        {
            return (int)value._value;
        }

        public static implicit operator Long(uint value)
        {
            return new Long(value);
        }

        public static implicit operator Long(int value)
        {
            return new Long((uint)value);
        }

        #endregion

        #region private

        private uint ReadUInt(ByteBuffer buffer)
        {
            buffer.ValidateRead(TypeSize);
            int data = ReadInt(buffer.Buffer, buffer.Offset);
            buffer.Complete(TypeSize);
            return (uint)data;
        }

        private int ReadInt(byte[] buffer, int offset)
        {
            return buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8 | buffer[offset + 3];
        }

        private int WriteInt(ByteBuffer buffer, uint data)
        {
            buffer.ValidateWrite(TypeSize);
            WriteInt(buffer.Buffer, buffer.WritePos, data);
            buffer.Append(TypeSize);
            return TypeSize;
        }

        private void WriteInt(byte[] buffer, int offset, uint data)
        {
            buffer[offset] = (byte)(data >> 24);
            buffer[offset + 1] = (byte)(data >> 16);
            buffer[offset + 2] = (byte)(data >> 8);
            buffer[offset + 3] = (byte)data;
        }

        #endregion

        public override string ToString()
        {
            return $"Long={_value}";
        }        
    }
}
