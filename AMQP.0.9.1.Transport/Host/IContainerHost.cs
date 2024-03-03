using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Host
{
    /// <summary>
    /// Represents an AMQP container that hosts processors.
    /// </summary>
    public interface IContainerHost : IContainer
    {
        /// <summary>
        /// Opens the _host object.
        /// </summary>
        Task OpenAsync(CancellationToken token = default);

        /// <summary>
        /// Closes the _host object.
        /// </summary>
        Task CloseAsync();
    }
}
