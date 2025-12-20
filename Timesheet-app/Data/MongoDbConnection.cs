using MongoDB.Driver;

namespace Timesheet_app.Data
{
    
    public class MongoDbConnection
    {
        private readonly IMongoDatabase _database;

        public MongoDbConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("timesheet");
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}
