using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    public abstract class BaseAmqpType<T>
    {
        #region fields

        protected T _value;
       
        #endregion

        #region Constructor

        protected BaseAmqpType(T value)
        {
            _value = value;
        }

        protected BaseAmqpType(ByteBuffer buffer)
        {
            _value = ReadFromBuffer(buffer);
        }

        #endregion

        #region protected

        protected abstract T ReadFromBuffer(ByteBuffer buffer);

        #endregion
    }
}
