using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using System.Text;

namespace AMQP_0_9_1.Types
{
    public class Shortstr : BaseAmqpType<string>, IAmqpType
    {
        #region Construstor

        private Shortstr(string value)
            : base(value)
        { 
        }

        private Shortstr(ByteBuffer value)
            : base(value)
        {

        }

        public static Shortstr Create(string value)
        {
            return new Shortstr(value);
        }

        public static Shortstr Create(ByteBuffer buffer)
        {
            return new Shortstr(buffer);
        }

        public static Shortstr Empty => new Shortstr("");

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            if (_value == null)
            {
                Octet.Create(FormatCode.Null).WriteTo(buffer);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(_value);

            Octet.Create((byte)bytes.Length).WriteTo(buffer);
            buffer.WriteBytes(bytes, 0, bytes.Length);
        }

        public int TypeSize => Octet.Empty.TypeSize + GetSize(_value);

        #endregion

        protected override string ReadFromBuffer(ByteBuffer buffer)
        {
            int length = Octet.Create(buffer);
            if (length == 0)
            {
                return string.Empty;
            }

            var bytes = new byte[length];
            buffer.ReadBytes(bytes, 0, length);

            return Encoding.UTF8.GetString(bytes);
        }

        #region

        #endregion

        #region implicit

        public static implicit operator string(Shortstr value)
        {
            return value?._value;
        }

        public static implicit operator Shortstr(string value)
        {
            return new Shortstr(value);
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
