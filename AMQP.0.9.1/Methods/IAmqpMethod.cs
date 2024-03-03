using AMQP_0_9_1.Types;

namespace AMQP_0_9_1.Framing
{
    /* 
    Method frames carry the high-level protocol commands (which we call "methods").
    One method frame carries one command.  The method frame payload has this format:

      0          2           4
      +----------+-----------+-------------- - -
      | class-id | method-id | arguments...
      +----------+-----------+-------------- - -
         short      short    ...

        See specification amqp-xml-doc0-9-1.pdf
    */
    public interface IAmqpMethod
    {
        Short ClassId { get; }

        Short MethodId { get; }

        Long GetPayloadLength();
    }
}