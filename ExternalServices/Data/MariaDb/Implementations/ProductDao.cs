using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb.Implementations
{
    public class ProductDao : Dao, IProductDao
    {
        public ProductDao(DatabaseConnector connector) : base(connector) { }

        public ICategoryDao Categories => new CategoryDao(this.Connector);

        public Product Create(Product item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            item.Id = db.InsertInto("Item")
                        .Value("name", item.Name)
                        .Value("isBundle", false)
                        .Value("isAvailable", (int)item.Availability)
                        .Value("currentStock", item.Stock)
                        .Value("category", item.Category.Id)
                        .Value("group", (object?)null)
                        .Value("picture", (object?)null)
                        .Value("deleted", false)
                        .Apply();

            db.InsertInto("Price")
                .Value("price", item.NonMemberPrice)
                .Value("isDiscount", false)
                .Value("effectiveDate", DateOnly.FromDateTime(DateTime.Now))
                .Value("expirationDate", (object?)null)
                .Value("expiresUponExhaustion", false)
                .Value("type",
                    db.Select("id").From("PricingType").Where(SQL.AND(
                        db.Column("applicableDuring") == 1,
                        db.Column("requiresMembership") == 0
                    ))
                )
                .Value("item", item.Id)
                .Apply();
            
            db.InsertInto("Price")
                .Value("price", item.MemberPrice)
                .Value("isDiscount", false)
                .Value("effectiveDate", DateOnly.FromDateTime(DateTime.Now))
                .Value("expirationDate", (object?)null)
                .Value("expiresUponExhaustion", false)
                .Value("type",
                    db.Select("id").From("PricingType").Where(SQL.AND(
                        db.Column("applicableDuring") == 1,
                        db.Column("requiresMembership") == 1
                    ))
                )
                .Value("item", item.Id)
                .Apply();
            
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Item").Set("deleted", true).Where(db.Column("id") == key).Apply();

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

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                WITH pv AS (
                  SELECT p.price, p.item, pt.requiresMembership FROM Price p JOIN PricingType pt ON p.type = pt.id AND pt.applicableDuring = 1
                )
                SELECT i.id AS productId
                     , i.name AS productName
                     , c.id AS categoryId
                     , c.name AS categoryName
                     , i.currentStock AS stock
                     , i.isAvailable AS availability
                     , nmp.price AS nonMemberPrice
                     , mp.price AS memberPrice
                FROM `Item` i
                JOIN Category c ON i.category = c.id
                JOIN pv nmp ON nmp.item = i.id AND nmp.requiresMembership = 0
                JOIN pv mp ON mp.item = i.id AND mp.requiresMembership = 1
                WHERE i.deleted = 0
            ";
            using var results = cmd.ExecuteReader();
            
            return this.ReadResults(results, Hydrate);
        }

        public Product Read(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                WITH pv AS (
                  SELECT p.price, p.item, pt.requiresMembership FROM Price p JOIN PricingType pt ON p.type = pt.id AND pt.applicableDuring = 1
                )
                SELECT i.id AS productId
                     , i.name AS productName
                     , c.id AS categoryId
                     , c.name AS categoryName
                     , i.currentStock AS stock
                     , i.isAvailable AS availability
                     , nmp.price AS nonMemberPrice
                     , mp.price AS memberPrice
                FROM `Item` i
                JOIN Category c ON i.category = c.id
                JOIN pv nmp ON nmp.item = i.id AND nmp.requiresMembership = 0
                JOIN pv mp ON mp.item = i.id AND mp.requiresMembership = 1
                WHERE i.deleted = 0 AND i.id = @key
            ";
            cmd.Parameters.AddWithValue("key", key);
            using var result = cmd.ExecuteReader();

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

            bool ok = db.Update("Item")
                        .Set("name", item.Name)
                        .Set("currentStock", item.Stock)
                        .Set("isAvailable", item.Availability)
                        .Set("category", item.Category.Id)
                        .Where(db.Column("id") == key)
                        .Apply();

            if (ok)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Price SET price = @price WHERE TYPE IN
                        (  SELECT id FROM PricingType WHERE 
                           applicableDuring = 1 AND
                           requiresMembership <> 1
                        ) AND
                        item = @item
                ";
                cmd.Parameters.AddWithValue("price", item.NonMemberPrice);
                cmd.Parameters.AddWithValue("item", key);
                cmd.ExecuteNonQuery();
                
                cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Price SET price = @price WHERE TYPE IN
                        (  SELECT id FROM PricingType WHERE 
                           applicableDuring = 1 AND
                           requiresMembership = 1
                        ) AND
                        item = @item
                ";
                cmd.Parameters.AddWithValue("price", item.MemberPrice);
                cmd.Parameters.AddWithValue("item", key);
                cmd.ExecuteNonQuery();
            }
            
            if (!ok) throw new ItemNotFoundException("Ce produit");

            item.Id = key;
            return item;
        }

        public void WithdrawFromStock(int id, int amount)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Item").Set("currentStock", db.Column("currentStock") - amount).Where(db.Column("id") == id).Apply();

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

            using (var result = db.Select(SQL.COUNT()).From("ProductImage").Where(db.Column("id") == id).Fetch())
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
