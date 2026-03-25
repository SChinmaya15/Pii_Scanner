using PiiScanner.Domain.Entity;
using PiiScanner.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiiScanner.Application.Service
{
    public class ComplianceEngine
    {
        private readonly IEnumerable<IComplianceRule> _rules;

        public ComplianceEngine(IEnumerable<IComplianceRule> rules)
        {
            _rules = rules;
        }

        public async Task<List<ComplianceResult>> EvaluateAsync(FileRecord record)
        {
            var results = new List<ComplianceResult>();

            foreach (var rule in _rules)
            {
                results.Add(await rule.EvaluateAsync(record));
            }

            return results;
        }
    }
}
