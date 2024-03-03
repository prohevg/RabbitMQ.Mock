using System;

namespace AMQP_0_9_1.Exceptions
{
    public class AmqpException : Exception
    {
        public AmqpException()
        {
        }

        public AmqpException(string error)
            : base(error)
        {
        }
    }
}
