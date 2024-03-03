using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Types
{
    public interface IAmqpType
    {
        void WriteTo(ByteBuffer buffer);

        int TypeSize { get; }
    }
}
