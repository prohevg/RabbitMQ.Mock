using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class QueueDeclareOk : IOutgoingAmqpMethod
    {
        public QueueDeclareOk(Shortstr queueName, Long messageCount, Long consumerCount)
        {
            QueueName = queueName;
            MessageCount = messageCount;
            ConsumerCount = consumerCount;
        }

        public Shortstr QueueName { get; set; }
        public Long MessageCount { get; set; } 
        public Long ConsumerCount {  get; set; }

        #region IOutgoingAmqpMethod

        public Short ClassId => Short.Create(50);

        public Short MethodId => Short.Create(11);

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            QueueName.WriteTo(buffer);
            MessageCount.WriteTo(buffer);
            ConsumerCount.WriteTo(buffer);
            return buffer.Length - length;
        }

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += QueueName.SizeOf();
            bufferSize += MessageCount.SizeOf();
            bufferSize += ConsumerCount.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion
    }
}
