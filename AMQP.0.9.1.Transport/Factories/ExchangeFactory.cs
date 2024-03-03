using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Host;
using AMQP_0_9_1.Transport.Mock;

namespace AMQP_0_9_1.Transport.Factories
{
    public class ExchangeFactory : IExchangeFactory
    {
        private readonly IQueueList _queueList;
        private readonly IMockCaseListHandler _mockCaseHandler;
        private readonly ContainerHostConfiguration _hostConfiguration;

        public ExchangeFactory(IQueueList queueList, IMockCaseListHandler mockCaseHandler, ContainerHostConfiguration hostConfiguration) 
        {
            _queueList = queueList;
            _mockCaseHandler = mockCaseHandler;
            _hostConfiguration = hostConfiguration;
        }

        public IExchange Create(string name)
        {
            return _hostConfiguration.UseMassTransit 
                ? new MasstransitExchange(name, _queueList, _mockCaseHandler) 
                : new Exchange(name, _queueList, _mockCaseHandler);
        }
    }
}