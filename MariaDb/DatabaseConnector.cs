using GalliumPlus.WebApi.Core.Exceptions;
using MySqlConnector;

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
            try
            {
                var connection = new MySqlConnection(this.connectionString);
                connection.Open();
                return connection;
            }
            catch (MySqlException error)
            {
                throw new ServiceUnavailableException($"La base de données est indisponible ({error.Message}).");
            }
        }
    }
}
