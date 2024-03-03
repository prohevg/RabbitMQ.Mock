using AMQP_0_9_1.Transport.Holders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Workers
{
    public class ConsumerPublisher : IConsumerPublisher
    {
        private readonly IQueueList _queueList;

        public ConsumerPublisher(IQueueList queueList) 
        {
            _queueList = queueList;
        }

        #region IConsumerPublisher

        public Task PublishAsync(CancellationToken token)
        {
            return Task.Factory.StartNew(async () =>
            {
                var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        foreach (var queue in _queueList.List.ToList())
                        {
                            try
                            {
                                await queue.HandleAsync(token);
                            }
                            catch (Exception ex)
                            {
                                AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AmqpTrace.WriteLine(AmqpTraceLevel.Error, ex.ToString());
                    }

                    await timer.WaitForNextTickAsync(token);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion
    }
}
