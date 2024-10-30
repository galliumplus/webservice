using MySqlConnector;

namespace GalliumPlus.Data.MariaDb
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
