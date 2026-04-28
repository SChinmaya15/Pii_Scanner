using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Domain.Interface
{
    public interface IScanService
    {
        Task<List<string>> CreateScan(string content);
    }
}
