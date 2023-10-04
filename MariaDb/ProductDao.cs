using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class ProductDao : Dao, IProductDao
    {
        public ProductDao(DatabaseConnector connector) : base(connector) { }

        public ICategoryDao Categories => new CategoryDao(this.Connector);

        public Product Create(Product item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            item.Id = db.InsertInto("Product")
                        .Value("@name", item.Name)
                        .Value("@stock", item.Stock)
                        .Value("@nonMemberPrice", item.NonMemberPrice)
                        .Value("@memberPrice", item.MemberPrice)
                        .Value("@availability", (int)item.Availability)
                        .Value("@category", item.Category.Id)
                        .Apply();

            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("Product").Where(db.Column("id") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Ce produit");
        }

        private static Product Hydrate(MySqlDataReader row)
        {
            return new Product(
                row.GetInt32("productId"),
                row.GetString("productName"),
                row.GetInt32("stock"),
                row.GetDecimal("nonMemberPrice"),
                row.GetDecimal("memberPrice"),
                (Availability)row.GetInt32("availability"),
                new Category(
                    row.GetInt32("categoryId"),
                    row.GetString("categoryName")
                )
            );
        }

        public IEnumerable<Product> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var productTable = db.Table("Product");
            var categoryTable = db.Table("Category");
            using var results = db.Select(productTable.Column("id").As("productId"), productTable.Column("name").As("productName"))
                                  .And(categoryTable.Column("id").As("categoryId"), categoryTable.Column("name").As("categoryName"))
                                  .And("stock", "nonMemberPrice", "memberPrice", "availability")
                                  .From(productTable)
                                  .Join(categoryTable.Column("id"), productTable.Column("category"))
                                  .Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }

        public Product Read(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var productTable = db.Table("Product");
            var categoryTable = db.Table("Category");
            using var result = db.Select(productTable.Column("id").As("productId"), productTable.Column("name").As("productName"))
                                 .And(categoryTable.Column("id").As("categoryId"), categoryTable.Column("name").As("categoryName"))
                                 .And("stock", "nonMemberPrice", "memberPrice", "availability")
                                 .From(productTable)
                                 .Join(categoryTable.Column("id"), productTable.Column("category"))
                                 .Where(productTable.Column("id") == key)
                                 .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce produit");
            }

            return Hydrate(result);
        }

        public Product Update(int key, Product item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Product")
                        .Set("name", item.Name)
                        .Set("stock", item.Stock)
                        .Set("nonMemberPrice", item.NonMemberPrice)
                        .Set("memberPrice", item.MemberPrice)
                        .Set("availability", item.Availability)
                        .Set("category", item.Category.Id)
                        .Where(db.Column("id") == key)
                        .Apply();
            
            if (!ok) throw new ItemNotFoundException("Ce produit");

            item.Id = key;
            return item;
        }

        public void WithdrawFromStock(int id, int amount)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Product").Set("stock", db.Column("stock") - amount).Where(db.Column("id") == id).Apply();

            if (!ok) throw new ItemNotFoundException("Ce produit");
        }

        public ProductImage ReadImage(int id)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var result = db.Select("imageSize", "image").From("ProductImage")
                                 .Where(db.Column("id") == id).Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("L'image de ce produit");
            }

            int size = result.GetInt32("imageSize");

            byte[] rawData = new byte[size];
            result.GetBytes("image", 0, rawData, 0, size);

            return ProductImage.FromStoredData(rawData);
        }

        public void SetImage(int id, ProductImage image)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool existingImage, ok;

            using (var result = db.Select(db.Count()).From("ProductImage").Where(db.Column("id") == id).Fetch())
            {
                existingImage = result.Read() && result.GetInt32(0) == 1;
            }

            if (existingImage)
            {
                ok = db.Update("ProductImage").Set("imageSize", image.Bytes.Length).Set("image", image.Bytes)
                       .Where(db.Column("id") == id).Apply();
            }
            else
            {
                try
                {
                    db.InsertInto("ProductImage").Value("id", id).Value("image", image.Bytes).Value("ImageSize").Apply();
                    ok = true;
                }
                catch (MySqlException err)
                {
                    // violation de clé étrangère
                    if (err.ErrorCode == MySqlErrorCode.NoReferencedRow2) ok = false;
                    else throw;
                }
            }

            if (!ok) throw new ItemNotFoundException("Ce produit");
        }

        public void UnsetImage(int id)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("ProductImage").Where(db.Column("id") == id).Apply();

            if (!ok) throw new ItemNotFoundException("L'image de ce produit");
        }
    }
}
