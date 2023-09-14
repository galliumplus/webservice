using MySqlConnector;
using System.Xml.Serialization;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class DatabaseConnector
    {
        private string? connectionString = null;

        public DatabaseConnector(string host, string userId, string password, string schema, uint port = 3306)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = host,
                Port = port,
                Database = schema,
                UserID = userId,
                Password = password,
                Pooling = true,
            };

            connectionString = builder.ConnectionString;
        }

        internal MySqlConnection Connect()
        {
            var connection = new MySqlConnection(this.connectionString);
            connection.Open();
            return connection;
        }
    }
}
