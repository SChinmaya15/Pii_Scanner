using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PiiScanner.Domain.Interface
{
    public interface IDataConnector
    {
        Task<Stream> ReadFileAsync(string path);
        Task<IEnumerable<string>> ListFilesAsync();
    }
}
