using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock.Interpretators
{
    public class ThanExpression : IExpression
    {
        private LinkedList<IExpression> _expressions;

        public ThanExpression(LinkedList<IExpression> expression)
        {
            _expressions = expression;
        }

        public async ValueTask<string> InterpretAsync(Context context)
        {
            foreach (var expression in _expressions)
            {
                var content = await expression.InterpretAsync(context);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return content;
                }
            }

            return null;
        }
    }
}
