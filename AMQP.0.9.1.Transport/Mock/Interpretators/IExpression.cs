using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock.Interpretators
{
    public interface IExpression
    {
        ValueTask<string> InterpretAsync(Context context);
    }
}
