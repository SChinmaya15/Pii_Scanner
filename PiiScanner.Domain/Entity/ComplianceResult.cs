using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PiiScanner.Domain.Entity
{
    public class ComplianceResult
    {
        public string? Message { get; set; }
        public string? RuleName { get; set; }
        public bool IsCompliant { get; set; }
    }
}
