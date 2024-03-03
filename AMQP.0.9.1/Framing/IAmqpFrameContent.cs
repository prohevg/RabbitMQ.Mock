using AMQP_0_9_1.Domain;

namespace AMQP_0_9_1.Framing
{
    /*
     Content Body

    +-----------------------+ +-----------+
    | Opaque binary payload | | frame-end |
    +-----------------------+ +-----------+

     See 4.2.6.2 The Content Body specification amqp-xml-doc0-9-1.pdf
    */
    public interface IAmqpFrameContent : IAmqpFrame
    {
        /// <summary>
        /// Body 
        /// </summary>
        ByteBuffer Payload { get; set; }

        /// <summary>
        /// Write to buffer
        /// </summary>
        ByteBuffer WriteTo();
    }
}