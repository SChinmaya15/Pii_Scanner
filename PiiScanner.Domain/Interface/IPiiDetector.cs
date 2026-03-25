using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Domain.Interface
{
    public interface IPiiDetector
    {
        Task<List<string>> DetectAsync(string content);
    }
}
