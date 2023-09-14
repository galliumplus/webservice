using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class CategoryDao : Dao, ICategoryDao
    {
        public CategoryDao(DatabaseConnector connector) : base(connector) { }

        public Category Create(Category item)
        {
            using var connection = this.Connect();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO `Category`(`name`) VALUES (@name)";
            insertCommand.Parameters.AddWithValue("@name", item.Name);

            insertCommand.ExecuteNonQuery();

            if (this.SelectLastInsertId(connection) is ulong id)
            {
                return item.WithId((int)id);
            }
            else
            {
                return item;
            }
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM `Category` WHERE `id` = @id";
            deleteCommand.Parameters.AddWithValue("@id", key);

            int affectedRows = deleteCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("catégorie", true);
            }
        }

        public IEnumerable<Category> Read()
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name` FROM `Category`";
            
            var results = readCommand.ExecuteReader();

            return this.ReadResults(results, row => new Category(
                row.GetInt32("id"),
                row.GetString("name")
            ));
        }

        public Category Read(int key)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name` FROM `Category` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", key);

            var result = readCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("catégorie", true);
            }

            return new Category(
                result.GetInt32("id"),
                result.GetString("name")
            );
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
                throw new ItemNotFoundException("catégorie", true);
            }

            return item.WithId(key);
        }
    }
}
