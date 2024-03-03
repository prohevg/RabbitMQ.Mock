namespace AMQP_0_9_1.Constants
{
    public static class BasicMethodConstants
    {
        public const ushort Qos = 10;
        public const ushort QosOk = 11;
        public const ushort Consume = 20;
        public const ushort ConsumeOk = 21;
        public const ushort Cancel = 30;
        public const ushort CancelOk = 31;
        public const ushort Publish = 40;
        public const ushort Return = 50;
        public const ushort Deliver = 60;
        public const ushort Get = 70;
        public const ushort GetOk = 71;
        public const ushort GetEmpty = 72;
        public const ushort Ack = 80;
        public const ushort Reject = 90;
        public const ushort RecoverAsync = 100;
        public const ushort Recover = 110;
        public const ushort RecoverOk = 111;
        public const ushort Nack = 120;
    }
}
