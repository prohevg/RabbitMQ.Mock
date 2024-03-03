using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock
{
    public interface IMockCaseListHandler
    {
        ValueTask LoadCasesListAsync(CancellationToken token = default);

        ValueTask<string> GetResponseAsync(string message);
    }
}