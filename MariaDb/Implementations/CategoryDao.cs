using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb.Implementations
{
    public class CategoryDao : Dao, ICategoryDao
    {
        public CategoryDao(DatabaseConnector connector) : base(connector) { }

        public Category Create(Category item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            
            int id = db.InsertInto("Category").Value("name", item.Name).Apply();

            return item.WithId(id);
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("Category").Where(db.Column("id") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Cette catégorie");
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
            Schema db = new(connection);

            using var results = db
                .Select("id", "name")
                .From("Category")
                .Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }

        public Category Read(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var result = db
                .Select("id", "name")
                .From("Category")
                .Where(db.Column("id") == key)
                .Fetch<MySqlDataReader>();

            if (!result.Read()) throw new ItemNotFoundException("Cette catégorie");
            
            return Hydrate(result);
        }

        public Category Update(int key, Category item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Category").Set("name", item.Name).Where(db.Column("id") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Cette catégorie");

            return item.WithId(key);
        }
    }
}
