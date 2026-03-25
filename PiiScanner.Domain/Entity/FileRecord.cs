using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Domain.Entity
{
    public class FileRecord
    {
        public Guid Id { get; set; }
        public bool HasPii { get; set; }
        public string? Path { get; set; }
        public string? Source { get; set; } // AWS, Azure, etc
        public DateTime LastScanned { get; set; }
        public List<string> PiiTypes { get; set; } = [];
    }
}
