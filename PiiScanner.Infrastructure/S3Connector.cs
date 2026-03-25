using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Amazon.S3;
using Amazon.S3.Model;
using PiiScanner.Domain.Interface;

namespace PiiScanner.Infrastructure
{
    public class S3Connector : IDataConnector
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Connector(IAmazonS3 s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        public async Task<IEnumerable<string>> ListFilesAsync()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            return response.S3Objects.Select(o => o.Key);
        }

        public async Task<Stream> ReadFileAsync(string key)
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            return response.ResponseStream;
        }
    }
}
