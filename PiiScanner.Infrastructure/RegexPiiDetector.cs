using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using PiiScanner.Domain.Interface;

namespace PiiScanner.Infrastructure
{
    public class RegexPiiDetector : IPiiDetector
    {
        public Task<List<string>> DetectAsync(string content)
        {
            var result = new List<string>();

            if (Regex.IsMatch(content, @"\b[A-Z]{5}[0-9]{4}[A-Z]\b"))
                result.Add("PAN");

            if (Regex.IsMatch(content, @"\b\d{12}\b"))
                result.Add("Aadhaar");

            if (Regex.IsMatch(content, @"\b\d{10}\b"))
                result.Add("Phone");

            if (Regex.IsMatch(content, @"\S+@\S+\.\S+"))
                result.Add("Email");

            return Task.FromResult(result);
        }
    }
}
