using AMQP_0_9_1.Domain;
using System;
using System.Linq;

namespace AMQP_0_9_1.Types
{
    public class AmqpBits : BaseAmqpType<byte>, IAmqpType
    {
        #region Constructor

        private AmqpBits(byte value)
            : base(value)
        {
        }

        private AmqpBits(ByteBuffer value)
            : base(value)
        {
        }

        public static AmqpBits Create(bool[] value)
        {
            if (value == null || !value.Any())
            {
                return new AmqpBits(0);
            }

            if (value.Length > 5)
            {
                throw new NotImplementedException();
            }

            var bytes = value.Select(ToByte).ToArray();
            int a = 0;
            int power = 1;
            foreach (var b in bytes)
            {
                a += b * power;
                power *= 2;
            }

            if (value.Length == 5)
            {
                return new AmqpBits((byte)(a | (bytes[4] << 4)));
            }

            return new AmqpBits((byte)a);
        }

        public static AmqpBits Create(ByteBuffer buffer)
        {
            return new AmqpBits(buffer);
        }

        public static AmqpBits Empty => new AmqpBits(0);

        #endregion

        #region public

        public void ReadValues(out bool val1)
        {
            val1 = Boolean.Create((sbyte)_value);
        }

        public void ReadValues(out bool val1, out bool val2)
        {
            val1 = (_value & 0b0000_0001) != 0;
            val2 = (_value & 0b0000_0010) != 0;
        }

        public void ReadValues(out bool val1, out bool val2, out bool val3)
        {
            val1 = (_value & 0b0000_0001) != 0;
            val2 = (_value & 0b0000_0010) != 0;
            val3 = (_value & 0b0000_0100) != 0;
        }

        public void ReadValues(out bool val1, out bool val2, out bool val3, out bool val4)
        {
            val1 = (_value & 0b0000_0001) != 0;
            val2 = (_value & 0b0000_0010) != 0;
            val3 = (_value & 0b0000_0100) != 0;
            val4 = (_value & 0b0000_1000) != 0;
        }

        public void ReadValues(out bool val1, out bool val2, out bool val3, out bool val4, out bool val5)
        {
            val1 = (_value & 0b0000_0001) != 0;
            val2 = (_value & 0b0000_0010) != 0;
            val3 = (_value & 0b0000_0100) != 0;
            val4 = (_value & 0b0000_1000) != 0;
            val5 = (_value & 0b0001_0000) != 0;
        }

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            Octet.Create(_value).WriteTo(buffer);
        }

        public int TypeSize => 1;

        #endregion

        #region override

        protected override byte ReadFromBuffer(ByteBuffer buffer)
        {
            return (byte)(sbyte)Octet.Create(buffer);
        }

        #endregion

        #region private

        private static byte ToByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        #endregion

        public override string ToString()
        {
            return $"AmqpBits={_value}";
        }
    }
}
