using ConsoleAppCrawlData.Configs;
using ConsoleAppCrawlData.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCrawlData.Services
{
    internal class JobsService
    {
        private readonly ILogger _logger;
        private readonly IMongoCollection<Jobs> _jobsCollection;

        public JobsService(ILogger<JobsService> logger,
            IOptions<JobStoreDatabaseSettings> jobStoreDatabaseSettings)
        {
            _logger = logger;
            var mongoClient = new MongoClient(
            jobStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                jobStoreDatabaseSettings.Value.DatabaseName);

            _jobsCollection = mongoDatabase.GetCollection<Jobs>(
                jobStoreDatabaseSettings.Value.JobsCollectionName);
        }

        public void Insert(Jobs input)
        {
            _jobsCollection.InsertOne(input);
        }

        public void DeleteAll()
        {
            _jobsCollection.DeleteMany(Builders<Jobs>.Filter.Empty);
        }
    }
}
