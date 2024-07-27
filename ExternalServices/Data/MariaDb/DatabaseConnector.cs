using GalliumPlus.WebApi.Core.Exceptions;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class DatabaseConnector
    {
        private string? connectionString = null;

        public DatabaseConnector(MariaDbOptions options)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = options.Host,
                Port = options.Port,
                Database = options.Schema,
                UserID = options.UserId,
                Password = options.Password,
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
