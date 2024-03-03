namespace AMQP_0_9_1.Constants
{
    public enum FrameType : byte
    {
        FrameMethod = AmqpConstants.FrameMethod,
        FrameHeader = AmqpConstants.FrameHeader,
        FrameBody = AmqpConstants.FrameBody,
        FrameHeartbeat = AmqpConstants.FrameHeartbeat
    }
}
