using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using PiiScanner.Domain.Entity;

namespace PiiScanner.Domain.Interface
{
    public interface IComplianceRule
    {
        string RuleName { get; }

        Task<ComplianceResult> EvaluateAsync(FileRecord record);
    }
}
