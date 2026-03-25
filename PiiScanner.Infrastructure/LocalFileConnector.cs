using PiiScanner.Domain.Interface;

namespace PiiScanner.Infrastructure
{
    public class LocalFileConnector : IDataConnector
    {
        private readonly string _rootPath;

        public LocalFileConnector(string rootPath)
        {
            _rootPath = rootPath;
        }

        public Task<IEnumerable<string>> ListFilesAsync()
        {
            var files = Directory.GetFiles(_rootPath, "*.*", SearchOption.AllDirectories);
            return Task.FromResult(files.AsEnumerable());
        }

        public Task<Stream> ReadFileAsync(string path)
        {
            Stream stream = File.OpenRead(path);
            return Task.FromResult(stream);
        }
    }
}
