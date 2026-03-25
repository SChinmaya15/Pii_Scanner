using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using PiiScanner.Domain.Entity;
using PiiScanner.Domain.Interface;

namespace PiiScanner.Application.Service
{
    public class PiiWithoutEncryptionRule : IComplianceRule
    {
        public string RuleName => "PII Without Encryption";

        public Task<ComplianceResult> EvaluateAsync(FileRecord record)
        {
            if (record.HasPii && !record.Path.Contains("encrypted"))
            {
                return Task.FromResult(new ComplianceResult
                {
                    RuleName = RuleName,
                    IsCompliant = false,
                    Message = "PII found in unencrypted file"
                });
            }

            return Task.FromResult(new ComplianceResult
            {
                RuleName = RuleName,
                IsCompliant = true
            });
        }
    }
}
