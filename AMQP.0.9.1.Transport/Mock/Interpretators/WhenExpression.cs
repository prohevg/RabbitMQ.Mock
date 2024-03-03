using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock.Interpretators
{
    public class WhenExpression : IExpression
    {
        private readonly IExpression _regexExpression;
        private readonly ThanExpression _thanExpression;

        public WhenExpression(IExpression regexExpression, ThanExpression thanExpression)
        {
            _regexExpression = regexExpression;
            _thanExpression = thanExpression;
        }

        public async ValueTask<string> InterpretAsync(Context context)
        {
            var match = await _regexExpression.InterpretAsync(context);
            if (!string.IsNullOrWhiteSpace(match))
            {
                return await _thanExpression.InterpretAsync(context);
            }

            return null;
        }
    }
}
