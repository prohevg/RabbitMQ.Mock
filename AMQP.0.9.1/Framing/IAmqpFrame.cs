using AMQP_0_9_1.Types;
using System;

namespace AMQP_0_9_1.Framing
{
    /*
     General Frame Format:

     +------------+---------+----------------+---------+------------------+
     | Frame Type | Channel | Payload length | Payload | Frame End Marker |
     +------------+---------+----------------+---------+------------------+
     | 1 byte     | 2 bytes | 4 bytes        | x bytes | 1 byte           |
     +------------+---------+----------------+---------+------------------+ 

     See 4.2.3 General Frame Format specification amqp-xml-doc0-9-1.pdf
    */
    public interface IAmqpFrame
    {
        /// <summary>
        /// Frame type
        /// AMQP defines these frame types:
        /// Type = 1, "METHOD": method frame
        /// Type = 2, "HEADER": content header frame
        /// Type = 3, "BODY": content body frame
        /// Type = 4, "HEARTBEAT": heartbeat frame
        /// </summary>
        Octet FrameType { get; set; }

        /// <summary>
        /// Channel Id
        /// </summary>
        Short ChannelId { get; set; }

        /// <summary>
        /// Payload length
        /// </summary>
        Long PayloadLength { get; set; }

        /// <summary>
        /// Read and parse payload 
        /// </summary>
        /// <param name="payload">Frame payload</param>
        void ReadPayload(byte[] payload);
    }
}