using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock.Interpretators
{
    public class RegexExpression : IExpression
    {
        private readonly string _pattern;

        public RegexExpression(string pattern)
        {
            _pattern = pattern;
        }

        public ValueTask<string> InterpretAsync(Context context)
        {
            var regex = new Regex(_pattern);
            if (regex.IsMatch(context.Message))
            {
                return ValueTask.FromResult("OK");
            }

            return ValueTask.FromResult(string.Empty);
        }
    }
}
