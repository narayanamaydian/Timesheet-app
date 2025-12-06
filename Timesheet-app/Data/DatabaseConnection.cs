using MongoDB.Driver;

namespace Timesheet_app.Data
{
    
    public class DatabaseConnection
    {
        private readonly IMongoDatabase _database;

        public DatabaseConnection(IConfiguration configuration)
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
