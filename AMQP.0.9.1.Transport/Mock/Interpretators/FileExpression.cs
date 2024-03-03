using System.IO;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Mock.Interpretators
{
    public class FileExpression : IExpression
    {
        private readonly string _filename;

        public FileExpression(string filename)
        {
            _filename = filename;
        }

        public async ValueTask<string> InterpretAsync(Context context)
        {
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, "Responses", _filename);
            if (!File.Exists(filePath))
            {
                throw new System.ArgumentNullException(nameof(filePath));
            }

            return await File.ReadAllTextAsync(filePath);
        }
    }
}
