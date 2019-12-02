using System.Configuration;
using System.Data;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace sellercatalogue.DAL
{
    public class DbFactory : IDbFactory
    {
        private readonly string _connectionString;
        private DatabaseSettings dbSettings;
        
        public DbFactory(IOptions<DatabaseSettings> dbOptions)
        {
            dbSettings = dbOptions.Value;
            _connectionString = dbSettings.ConnectionString;
        }
        
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}