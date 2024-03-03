using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using System.Text;

namespace AMQP_0_9_1.Types
{
    public class Longstr : BaseAmqpType<string>, IAmqpType
    {
        #region Constructor

        private Longstr(string value)
            : base(value)
        { 
        }

        private Longstr(ByteBuffer value)
            : base(value)
        {
        }

        public static Longstr Create(string value)
        {
            return new Longstr(value);
        }

        public static Longstr Create(ByteBuffer buffer)
        {
            return new Longstr(buffer);
        }

        public static Longstr Empty => new Longstr((string)null);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            if (_value == null)
            {
                Long.Create(0).WriteTo(buffer);
               // LongUint.Create(FormatCode.Null).WriteTo(buffer); 
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(_value);

            Long.Create(bytes.Length).WriteTo(buffer);
            buffer.WriteBytes(bytes, 0, bytes.Length);
        }

        public int TypeSize => Long.Empty.SizeOf() + GetSize(_value);

        #endregion

        #region

        protected override string ReadFromBuffer(ByteBuffer buffer)
        {
            var length = Long.Create(buffer);
            if (length == 0)
            {
                return string.Empty;
            }

            var val = new byte[length];
            buffer.ReadBytes(val, 0, length);

            return Encoding.UTF8.GetString(val);
        }

        #endregion

        #region implicit

        public static implicit operator string(Longstr value)
        {
            return value?._value;
        }

        public static implicit operator Longstr(string value)
        {
            return new Longstr(value);
        }

        #endregion

        #region private

        private int GetSize(string value)
        {
            return string.IsNullOrEmpty(value) 
                ? 0 
                : Encoding.UTF8.GetByteCount(value);
        }

        #endregion

        public override string ToString()
        {
            return _value;
        }
    }
}
