using System;

namespace AMQP_0_9_1.Exceptions
{
    public class AmqpReaderException : Exception
    {
        public AmqpReaderException(string error)
            : base(error)
        {
        }
    }
}
