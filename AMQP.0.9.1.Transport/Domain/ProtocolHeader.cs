using AMQP_0_9_1.Exceptions;
using System.Text;

namespace AMQP_0_9_1.Transport.Domain
{
    public struct ProtocolHeader
    {
        public byte Id;

        public byte Major;

        public byte Minor;

        public byte Revision;

        public static ProtocolHeader Create(byte[] buffer, int offset)
        {
            if (buffer[offset + 0] != (byte)'A' ||
                buffer[offset + 1] != (byte)'M' ||
                buffer[offset + 2] != (byte)'Q' ||
                buffer[offset + 3] != (byte)'P')
            {
                throw new AmqpException("ProtocolName Expect:AMQP Actual:" + new string(Encoding.UTF8.GetChars(buffer, offset, 4)));
            }

            return new ProtocolHeader()
            {
                Id = buffer[offset + 4],
                Major = buffer[offset + 5],
                Minor = buffer[offset + 6],
                Revision = buffer[offset + 7]
            };
        }

#if TRACE
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Id, Major, Minor, Revision);
        }
#endif
    }
}
