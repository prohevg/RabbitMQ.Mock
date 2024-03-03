using AMQP_0_9_1.Domain;
using System;

namespace AMQP_0_9_1.Types
{
    /// <summary>
    /// short-uint = 2*OCTET
    /// ushort c#
    /// </summary>
    public class Short : BaseAmqpType<ushort>, IAmqpType
    {
        #region Construstor

        private Short(ushort value)
            : base(value) 
        {
        }

        private Short(ByteBuffer value)
            : base(value)
        {

        }

        public static Short Create(ushort value)
        {
            return new Short(value);
        }

        public static Short Create(short value)
        {
            return new Short((ushort)value);
        }

        public static Short Create(ByteBuffer value)
        {
            return new Short(value);
        }
        
        public static Short Empty => new Short(0);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            WriteShort(buffer, _value);
        }

        public int TypeSize => 2;

        #endregion

        #region override

        protected override ushort ReadFromBuffer(ByteBuffer buffer)
        {
            return ReadShort(buffer);
        }

        #endregion

        #region private

        /// <summary>
        /// Reads a 16-bit signed integer and advance the buffer _read cursor.
        /// </summary>
        /// <param name="buffer">the buffer to _read from.</param>
        /// <returns></returns>
        private ushort ReadShort(ByteBuffer buffer)
        {
            buffer.ValidateRead(TypeSize);
            ushort data = (ushort)(buffer.Buffer[buffer.Offset] << 8 | buffer.Buffer[buffer.Offset + 1]);
            buffer.Complete(TypeSize);
            return data;
        }

        /// <summary>
        /// Writes a 16-bit signed integer into the buffer and advance the buffer write cursor.
        /// </summary>
        /// <param name="buffer">the buffer to write.</param>
        /// <param name="data">The data to write.</param>
        private int WriteShort(ByteBuffer buffer, ushort data)
        {
            buffer.ValidateWrite(TypeSize);
            buffer.Buffer[buffer.WritePos] = (byte)(data >> 8);
            buffer.Buffer[buffer.WritePos + 1] = (byte)data;
            buffer.Append(TypeSize);
            return TypeSize;
        }

        #endregion

        #region implicit

        public static implicit operator ushort(Short value)
        {
            return value?._value ?? 0;
        }

        public static implicit operator Octet(Short value)
        {
            return new Short(value);
        }

        #endregion

        public override string ToString()
        {
            return $"Short={_value}";
        }
    }
}
