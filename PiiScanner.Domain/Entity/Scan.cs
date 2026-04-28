using PiiScanner.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Domain.Entity
{
    public class Scan
    {
        public string Name { get; set; }

        public StorageSource Location { get; set; }

        // Store extensions as a list for better handling
        public List<string> Extensions { get; set; }

        public Frequency Frequency { get; set; }

        public ActionType Action { get; set; }

        public string ApiKey { get; set; }

        public string SecretKey { get; set; }
    }
}
