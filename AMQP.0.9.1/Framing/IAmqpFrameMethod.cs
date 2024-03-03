using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Methods;

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
    
      See 4.2.4 Method Payloads specification amqp-xml-doc0-9-1.pdf
    */
    public interface IAmqpFrameMethod : IAmqpFrame
    {
        /// <summary>
        /// Payload
        /// </summary>
        ByteBuffer Payload { get; set; }

        /// <summary>
        /// true - if need to read next frame (expected body)
        /// </summary>
        bool IsExpectedBody { get; }
       
        /// <summary>
        /// Write to buffer
        /// </summary>
        /// <param name="amqpMethod">Method</param>
        /// <returns>Buffer</returns>
        ByteBuffer WriteTo(IOutgoingAmqpMethod amqpMethod);

        /// <summary>
        /// Write to buffer
        /// </summary>
        /// <returns>Buffer</returns>
        ByteBuffer WriteTo();
    }
}