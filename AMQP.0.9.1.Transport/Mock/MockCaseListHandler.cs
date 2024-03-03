using AMQP_0_9_1.Transport.Mock.Interpretators;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock
{
    public class MockCaseListHandler : IMockCaseListHandler
    {
        private readonly Dictionary<string, IExpression> _expressions = new();

        #region IMockCaseHandler

        public async ValueTask LoadCasesListAsync(CancellationToken token = default)
        {
            var path = Directory.GetCurrentDirectory();
            var directory = Path.Combine(path, "Cases");

            foreach (var filename in Directory.EnumerateFiles(directory))
            {
                var content = await File.ReadAllTextAsync(filename, token);
                var jObject = JObject.Parse(content);

                var whenCase = WhenCase(jObject);
                var thanList = ThanList(jObject);

                _expressions.Add(filename, new WhenExpression(whenCase, new ThanExpression(thanList)));
            }
        }

        public async ValueTask<string> GetResponseAsync(string message)
        {
            var context = new Context
            {
                Message = message
            };

            foreach (var expression in _expressions)
            {
                var response = await expression.Value.InterpretAsync(context);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    AmqpTrace.WriteLine(AmqpTraceLevel.Information, $"Response read from {expression.Key}");
                    return response;
                }
            }

            return null;
        }

        #endregion

        #region private

        private LinkedList<IExpression> ThanList(JObject jObject)
        {
            var than = new LinkedList<IExpression>();
            foreach (JObject jThan in jObject.Property("Than").Children())
            {
                if (jThan.Property("File") != null)
                {
                    var file = jThan.Property("File").Value.ToString();
                    than.AddLast(new FileExpression(file));
                }
            }
            return than;
        }

        private IExpression WhenCase(JObject jObject)
        {
            foreach (JObject jWhen in jObject.Property("When").Children())
            {
                if (jWhen.Property("Regex") != null)
                {
                    var pattern = jWhen.Property("Regex").Value.ToString();
                    return new RegexExpression(pattern);
                }

                throw new System.NotImplementedException(jWhen.Path);
            }

            throw new System.NotImplementedException(jObject.Path);
        }

        #endregion
    }
}
