using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using PiiScanner.Domain.Entity;
using PiiScanner.Domain.Entity.Request;

namespace PiiScanner.Infrastructure
{
    public class MongoRepo
    {
        private readonly IMongoCollection<Scan> _scans;
        private readonly IMongoCollection<Login> _logins;

        public MongoRepo(IConfiguration cfg)
        {
            var settings = MongoClientSettings.FromConnectionString(cfg["Mongo:ConnectionString"]);
            // Use Stable API version
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var dbName = cfg["Mongo:Database"] ?? "piiscanner";
            var db = client.GetDatabase(dbName);

            _scans = db.GetCollection<Scan>("scans");
            _logins = db.GetCollection<Login>("logins");
        }

        // Scan methods
        public Task CreateScanAsync(Scan scan) => _scans.InsertOneAsync(scan);

        public Task<List<Scan>> GetScansAsync() => _scans.Find(_ => true).ToListAsync();

        // Look up by the "Name" field using a string-based filter to avoid compile-time dependency on an Id property
        public Task<Scan?> GetScanByNameAsync(string name) =>
        _scans.Find(Builders<Scan>.Filter.Eq("Name", name)).FirstOrDefaultAsync();

        public Task UpdateScanAsync(Scan scan)
        {
            var filter = Builders<Scan>.Filter.Eq("Name", scan.Name);
            return _scans.ReplaceOneAsync(filter, scan, new ReplaceOptions { IsUpsert = true });
        }

        public Task DeleteScanByNameAsync(string name) =>
        _scans.DeleteOneAsync(Builders<Scan>.Filter.Eq("Name", name));

        public Task<long> GetScanCountAsync() => _scans.CountDocumentsAsync(_ => true);

        // Login methods
        public Task CreateLoginAsync(Login login) => _logins.InsertOneAsync(login);

        // Generic field lookup for login (e.g. by "Username" or "Email"). Field name is used as-is in the document.
        public Task<Login?> GetLoginByFieldAsync(string fieldName, string value) =>
        _logins.Find(Builders<Login>.Filter.Eq(fieldName, value)).FirstOrDefaultAsync();

        public Task<List<Login>> GetAllLoginsAsync() => _logins.Find(_ => true).ToListAsync();

        public Task<long> GetLoginCountAsync() => _logins.CountDocumentsAsync(_ => true);
    }
}
