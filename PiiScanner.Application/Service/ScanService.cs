using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using PiiScanner.Domain.Entity;
using PiiScanner.Domain.Interface;

namespace PiiScanner.Application.Service
{
    public class ScanService
    {
        private readonly IPiiDetector _piiDetector;
        private readonly IDataConnector _connector;
        
        public ScanService(IDataConnector connector, IPiiDetector piiDetector)
        {
            _connector = connector;
            _piiDetector = piiDetector;
        }

        public async Task<List<FileRecord>> ScanAsync()
        {
            var results = new List<FileRecord>();
            var files = await _connector.ListFilesAsync();

            foreach (var file in files)
            {
                using var stream = await _connector.ReadFileAsync(file);
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                var pii = await _piiDetector.DetectAsync(content);

                results.Add(new FileRecord
                {
                    Id = Guid.NewGuid(),
                    Path = file,
                    PiiTypes = pii,
                    HasPii = pii.Count != 0,
                    LastScanned = DateTime.UtcNow
                });
            }

            return results;
        }

        // Implement CreateScan to conform to IScanService (returns list of detected pii strings)
        public async Task<List<string>> CreateScan(Scan scan)
        {
            // For now, perform a simple detection on the scan.Name and return detected PII types
            var contentToScan = scan.Name ?? string.Empty;
            var detected = await _piiDetector.DetectAsync(contentToScan);

            // In a fuller implementation you might persist a Scan document via a repository
            return detected;
        }
    }
}
