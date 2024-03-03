using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    public class Octet : BaseAmqpType<sbyte>, IAmqpType
    {
        #region Construstor

        private Octet(sbyte value)
            : base(value) 
        {
        }

        private Octet(ByteBuffer value)
            : base(value)
        {

        }

        public static Octet Create(byte value)
        {
            return new Octet((sbyte)value);
        }

        public static Octet Create(sbyte value)
        {
            return new Octet(value);
        }

        public static Octet Create(char value)
        {
            return new Octet((sbyte)value);
        }

        public static Octet Create(ByteBuffer buffer)
        {
            return new Octet(buffer);
        }

        public static Octet Empty => new Octet(0);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            WriteByte(buffer, _value);
        }

        public int TypeSize => 1;

        #endregion

        #region

        protected override sbyte ReadFromBuffer(ByteBuffer buffer)
        {
            return ReadByte(buffer);
        }

        #endregion

        #region private

        private sbyte ReadByte(ByteBuffer buffer)
        {
            buffer.ValidateRead(TypeSize);
            sbyte data = (sbyte)buffer.Buffer[buffer.Offset];
            buffer.Complete(TypeSize);
            return data;
        }

        private int WriteByte(ByteBuffer buffer, sbyte data)
        {
            buffer.ValidateWrite(TypeSize);
            buffer.Buffer[buffer.WritePos] = (byte)data;
            buffer.Append(TypeSize);
            return TypeSize;
        }

        #endregion

        #region implicit

        public static implicit operator sbyte(Octet value)
        {
            return value?._value ?? 0;
        }

        public static implicit operator Octet(byte value)
        {
            return Create(value);
        }

        #endregion

        public override string ToString()
        {
            return $"byte={_value}, char={(char)_value}";
        }        
    }
}
