using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OTest.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<BsonDocument> _tokensCollection;

        private readonly IMongoCollection<BsonDocument> _usersCollection;
        private readonly ILogger<MongoDbService> _logger;

        public MongoDbService(IConfiguration configuration, ILogger<MongoDbService> logger)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("OTest");
            _usersCollection = database.GetCollection<BsonDocument>("Users");
            _logger = logger;
        }

        public async Task StoreUsers(string usersJson)
        {
            try
            {
                var document = BsonDocument.Parse(usersJson);
                await _usersCollection.InsertOneAsync(document);
                _logger.LogInformation("Stored users data in MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while storing users data in MongoDB.");
            }
        }

        public async Task StoreUser(string userJson)
        {
            try
            {
                var document = BsonDocument.Parse(userJson);
                await _usersCollection.InsertOneAsync(document);
                _logger.LogInformation("Stored a new user in MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while storing a new user in MongoDB.");
            }
        }

        public async Task<List<BsonDocument>> GetUsers()
        {
            try
            {
                var users = await _usersCollection.Find(new BsonDocument()).ToListAsync();
                _logger.LogInformation("Retrieved users from MongoDB.");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users from MongoDB.");
                return new List<BsonDocument>();
            }
        }

        public async Task AddUser(BsonDocument userDocument)
        {
            try
            {
                await _usersCollection.InsertOneAsync(userDocument);
                _logger.LogInformation("Added a new user to MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new user to MongoDB.");
            }
        }
        public async Task StoreToken(string tokenJson)
        {
            var document = BsonDocument.Parse(tokenJson);
            await _tokensCollection.InsertOneAsync(document);
        }
    }
}
