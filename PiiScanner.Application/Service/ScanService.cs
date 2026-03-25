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
    }
}
