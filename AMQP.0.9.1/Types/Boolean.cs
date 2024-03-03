using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    public class Boolean : BaseAmqpType<sbyte>, IAmqpType
    {
        #region Constructor

        private Boolean(sbyte value)
            : base(value) 
        {
        }

        private Boolean(ByteBuffer value) 
            : base(value)
        {
        }

        public static Boolean Create(sbyte value)
        {
            return new Boolean(value);
        }

        public static Boolean Create(bool value)
        {
            return new Boolean(value ? (sbyte)1 : (sbyte)0);
        }

        public static Boolean Create(ByteBuffer buffer)
        {
            return new Boolean(buffer);
        }

        public static Boolean Empty => new Boolean(0);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            Octet.Create(_value).WriteTo(buffer);
        }

        public int TypeSize => 1;

        #endregion

        #region implicit

        public static implicit operator sbyte(Boolean value)
        {
            return value?._value ?? 0;
        }
        public static implicit operator bool(Boolean value)
        {
            return value?._value == 1;
        }

        public static implicit operator Boolean(sbyte value)
        {
            return new Boolean(value);
        }

        public static implicit operator Boolean(byte value)
        {
            return new Boolean((sbyte)value);
        }

        #endregion

        #region override

        protected override sbyte ReadFromBuffer(ByteBuffer buffer)
        {
            return (sbyte)Octet.Create(buffer);
        }

        #endregion

        public override string ToString()
        {
            return $"Boolean={_value}";
        }
    }
}
