using GalliumPlus.Core.Exceptions;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb
{
    public class DatabaseConnector
    {
        private readonly string? connectionString;

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

            this.connectionString = builder.ConnectionString;
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
