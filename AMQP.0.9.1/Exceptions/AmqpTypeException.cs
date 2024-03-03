using System;

namespace AMQP_0_9_1.Exceptions
{
    public class AmqpTypeException : Exception
    {
        public AmqpTypeException(string error)
            : base(error)
        {
        }
    }
}
