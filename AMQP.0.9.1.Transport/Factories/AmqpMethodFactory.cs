using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Methods;
using AMQP_0_9_1.Methods.Incoming;
using AMQP_0_9_1.Types;
using System;

namespace AMQP_0_9_1.Transport.Factories
{
    public class AmqpMethodFactory : IAmqpMethodFactory
    {
        #region IMethodFactory

        public IIncomingAmqpMethod CreateIncomingMethod(IAmqpFrameMethod frame)
        {
            frame.Payload.Seek(0);

            var classId = Short.Create(frame.Payload);
            var methodId = Short.Create(frame.Payload);
            var method = CreateIncomingMethod(classId, methodId);

            method.ReadTo(frame.Payload);

            return method;
        }

        #endregion

        #region private

        private IIncomingAmqpMethod CreateIncomingMethod(Short classId, Short methodId)
        {
            var commandId = (AmpqCommandId)(classId << 16 | methodId);

            switch (commandId)
            {
                case AmpqCommandId.ConnectionStartOk:
                    return new ConnectionStartOk();
                case AmpqCommandId.ConnectionTuneOk:
                    return new ConnectionTuneOk();
                case AmpqCommandId.ConnectionOpen:
                    return new ConnectionOpen();
                case AmpqCommandId.ConnectionClose:
                    return new ConnectionClose();

                case AmpqCommandId.ChannelOpen:
                    return new ChannelOpen();
                case AmpqCommandId.ChannelClose:
                    return new ChannelClose();

                case AmpqCommandId.ExchangeDeclare:
                    return new ExchangeDeclare();
                case AmpqCommandId.ExchangeBind:
                    return new ExchangeBind();

                case AmpqCommandId.QueueDeclare:
                    return new QueueDeclare();
                case AmpqCommandId.QueueBind:
                    return new QueueBind();

                case AmpqCommandId.BasicAck:
                    return new FromClientBasicAsk();
                case AmpqCommandId.BasicQos:
                    return new BasicQos();
                case AmpqCommandId.BasicPublish:
                    return new BasicPublish();
                case AmpqCommandId.BasicConsume:
                    return new BasicConsume();


                case AmpqCommandId.ConfirmSelect:
                    return new ConfirmSelect();
            }

            AmqpTrace.WriteLine(AmqpTraceLevel.Frame, $"CreateIncomingMethod. ClassId: {classId}, methodId: {methodId}. See specification amqp-xml-doc0-9-1.pdf");
            throw new NotImplementedException($"CreateIncomingMethod. ClassId: {classId}, methodId: {methodId}. See sprecification amqp-xml-doc0-9-1.pdf");
        }       

        #endregion
    }
}
