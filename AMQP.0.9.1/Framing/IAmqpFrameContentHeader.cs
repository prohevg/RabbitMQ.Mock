using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    /*
        Content header
        0          2        4           12               14
        +----------+--------+-----------+----------------+------------- - -
        | class-id | weight | body size | property flags | property list...
        +----------+--------+-----------+----------------+------------- - -
         short        short   long long       short          remainder...

        See  4.2.6.1 The Content Header specification amqp-xml-doc0-9-1.pdf
     */
    public interface IAmqpFrameContentHeader : IAmqpFrame
    {
        /// <summary>
        /// Class Id
        /// </summary>
        Short ClassId { get; set; }

        /// <summary>
        /// Weight
        /// </summary>
        Short Weight { get; set; }

        /// <summary>
        /// Body size
        /// </summary>
        LongLong BodySize { get; set; }

        /// <summary>
        /// Properties
        /// </summary>
        IBasicProperties BasicProperties { get; set; }

        /// <summary>
        /// Write to buffer
        /// </summary>
        ByteBuffer WriteTo();
    }
}