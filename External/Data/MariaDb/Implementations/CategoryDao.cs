using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Stocks;
using KiwiQuery;
using KiwiQuery.Mapped;
using KiwiQuery.Mapped.Exceptions;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class CategoryDao : Dao, ICategoryDao
    {
        public CategoryDao(DatabaseConnector connector) : base(connector)
        {
        }

        public Category Create(Category category)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            
            return db.Table<int, Category>().InsertOne(category);
        }

        public IEnumerable<Category> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            
            return db.Table<int, Category>().SelectAll();
        }

        public Category Read(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            
            try
            {
                return db.Table<int, Category>().SelectOne(key);
            }
            catch (NotFoundException)
            {
                throw new ItemNotFoundException("Cette catégorie");
            }
        }

        public Category Update(int key, Category category)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            if (db.Update<Category>().SetOnly(category, "name").Where(db.Column("id") == key).Apply())
            {
                return db.Table<int, Category>().SelectOne(key);
            }
            else
            {
                throw new ItemNotFoundException("Cette catégorie");
            }
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            if (!db.Table<int, Category>().DeleteOne(key))
            {
                throw new ItemNotFoundException("Cette catégorie");
            }
        }
    }
}