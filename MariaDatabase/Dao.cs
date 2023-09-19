using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public abstract class Dao
    {
        private DatabaseConnector connector;

        public DatabaseConnector Connector => this.connector;

        public Dao(DatabaseConnector connector)
        {
            this.connector = connector;
        }

        protected MySqlConnection Connect() => this.connector.Connect();

        protected ulong SelectLastInsertId(MySqlConnection connection)
        {
            var selectIdCommand = connection.CreateCommand();
            selectIdCommand.CommandText = "SELECT LAST_INSERT_ID()";
            return (ulong)selectIdCommand.ExecuteScalar()!;
        }

        protected IEnumerable<T> ReadResults<T>(MySqlDataReader reader, Func<MySqlDataReader, T> hydration)
        {
            List<T> results = new();
            while (reader.Read())
            {
                results.Add(hydration(reader));
            }
            return results;
        }
    }
}
