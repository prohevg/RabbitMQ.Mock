using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Domain
{
    public interface IExchange
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Bind queue to exchange
        /// </summary>
        /// <param name="queue">Queue name</param>
        /// <param name="routingKey">Routing key</param>
        void BindQueue(string queue, string routingKey);

        /// <summary>
        /// Add message to queue
        /// </summary>
        /// <param name="routingKey">Routing key</param>
        /// <param name="content">Content</param>
        ValueTask RouteToAsync(string routingKey, LinkedList<IAmqpFrameContent> content);
    }
}
