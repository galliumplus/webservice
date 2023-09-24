using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class CategoryDao : Dao, ICategoryDao
    {
        public CategoryDao(DatabaseConnector connector) : base(connector) { }

        public Category Create(Category item)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `Category`(`name`) VALUES (@name)";
            command.Parameters.AddWithValue("@name", item.Name);

            command.ExecuteNonQuery();

            return item.WithId((int)this.SelectLastInsertId(connection));
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `Category` WHERE `id` = @id";
            command.Parameters.AddWithValue("@id", key);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette catégorie");
            }
        }

        private static Category Hydrate(MySqlDataReader row)
        {
            return new Category(
                row.GetInt32("id"),
                row.GetString("name")
            );
        }

        public IEnumerable<Category> Read()
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name` FROM `Category`";

            using var results = readCommand.ExecuteReader();

            return this.ReadResults(results, Hydrate);
        }

        public Category Read(int key)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name` FROM `Category` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", key);

            using var result = readCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette catégorie");
            }

            return Hydrate(result);
        }

        public Category Update(int key, Category item)
        {
            using var connection = this.Connect();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE `Category` SET `name` = @name WHERE `id` = @id";
            updateCommand.Parameters.AddWithValue("@name", item.Name);
            updateCommand.Parameters.AddWithValue("@id", key);

            int affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette catégorie");
            }

            return item.WithId(key);
        }
    }
}
