using AMQP_0_9_1.Listener;
using AMQP_0_9_1.Transport.Domain;
using AMQP_0_9_1.Transport.Factories;
using AMQP_0_9_1.Transport.Holders;
using AMQP_0_9_1.Transport.Mock;
using AMQP_0_9_1.Transport.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Host
{
    public class ContainerHost : IContainerHost
    {
        #region fields

        private readonly List<IConnectionListener> _listeners;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection;
        private readonly IConnectionListenerFactory _listenerFactory;
        private readonly IConsumerPublisher _consumerPublisher;
        private readonly IMockCaseListHandler _casesHandler;

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a container host object with one address.
        /// <param name="address">The address.</param>
        /// </summary>
        public ContainerHost(Address address, Action<ContainerHostConfiguration> cfg = null)
            : this(new[] { address }, cfg)
        {
        }

        /// <summary>
        /// Initializes a container host object with multiple addresses.
        /// <param name="addressList">The list of listen addresses.</param>
        /// </summary>
        public ContainerHost(IList<Address> addressList, Action<ContainerHostConfiguration> cfg = null)
        {
            var configuration = new ContainerHostConfiguration();

            cfg?.Invoke(configuration);

            _serviceCollection = FillServiceCollection(configuration);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            _listenerFactory = _serviceProvider.GetRequiredService<IConnectionListenerFactory>();
            _consumerPublisher = _serviceProvider.GetRequiredService<IConsumerPublisher>();
            _casesHandler = _serviceProvider.GetRequiredService<IMockCaseListHandler>();

            _listeners = new List<IConnectionListener>(addressList.Count);

            foreach (var address in addressList)
            {
                _listeners.Add(_listenerFactory.Create(address));
            }
        }

        #endregion

        #region IContainerHost

        public async Task OpenAsync(CancellationToken token = default)
        {
            _ = _consumerPublisher.PublishAsync(token).ConfigureAwait(false);

            await _casesHandler.LoadCasesListAsync(token).ConfigureAwait(false);

            foreach (var listener in _listeners)
            {
                await listener.OpenAsync(token).ConfigureAwait(false);
            }
        }

        public async Task CloseAsync()
        {
            foreach (var listener in _listeners)
            {
                try
                {
                    await listener.CloseAsync();
                }
                catch (Exception ex)
                {
                    AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.ToString());
                }
            }
        }

        #endregion

        private IServiceCollection FillServiceCollection(ContainerHostConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton(configuration.Tcp);
            serviceCollection.AddSingleton(configuration.Connection);


            serviceCollection.AddScoped<IAmqpMethodFactory, AmqpMethodFactory>();

            serviceCollection.AddScoped<IAddressFactory, AddressFactory>();

            serviceCollection.AddScoped<IConnectionListenerFactory, ConnectionListenerFactory>();
            serviceCollection.AddScoped<IConnectionListener, ConnectionListener>();

            serviceCollection.AddScoped<ITransportListenerFactory, TransportListenerFactory>();

            serviceCollection.AddScoped<IConnectionFactory, ConnectionFactory>();
            serviceCollection.AddScoped<IChannelFactory, ChannelFactory>();
            serviceCollection.AddScoped<IExchangeFactory, ExchangeFactory>();
            serviceCollection.AddScoped<IQueueFactory, QueueFactory>();

            serviceCollection.AddSingleton<IConnectionList, ConnectionList>();
            serviceCollection.AddSingleton<IChannelList, ChannelList>();
            serviceCollection.AddSingleton<IExchangeList, ExchangeList>();
            serviceCollection.AddSingleton<IQueueList, QueueList>();

            serviceCollection.AddSingleton<IConsumerPublisher, ConsumerPublisher>();

            serviceCollection.AddSingleton<IMockCaseListHandler, MockCaseListHandler>();

            return serviceCollection;
        }
    }
}
