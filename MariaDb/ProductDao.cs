using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
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

            var command = connection.CreateCommand();
            command.CommandText
                = "INSERT INTO `Product`(`name`, `stock`, `nonMemberPrice`, `memberPrice`, `availability`, `category`) "
                + "VALUES (@name, @stock, @nonMemberPrice, @memberPrice, @availability, @category)";
            command.Parameters.AddWithValue("@name", item.Name);
            command.Parameters.AddWithValue("@stock", item.Stock);
            command.Parameters.AddWithValue("@nonMemberPrice", item.NonMemberPrice);
            command.Parameters.AddWithValue("@memberPrice", item.MemberPrice);
            command.Parameters.AddWithValue("@availability", (int)item.Availability);
            command.Parameters.AddWithValue("@category", item.Category.Id);

            command.ExecuteNonQuery();

            item.Id = (int)this.SelectLastInsertId(connection);
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `Product` WHERE `id` = @id";
            command.Parameters.AddWithValue("@id", key);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Ce produit");
            }
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

            var readCommand = connection.CreateCommand();
            readCommand.CommandText
                = "SELECT `Product`.`id` as `productId`, `Product`.`name` as `productName`, "
                + "`stock`, `nonMemberPrice`, `memberPrice`, `availability`, "
                + "`Category`.`id` as `categoryId`, `Category`.`name` as `categoryName` "
                + "FROM `Product` INNER JOIN `Category` ON `Category`.`id` = `Product`.`category`";

            using var results = readCommand.ExecuteReader();

            return this.ReadResults(results, Hydrate);
        }

        public Product Read(int key)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText
                = "SELECT `Product`.`id` as `productId`, `Product`.`name` as `productName`, "
                + "`stock`, `nonMemberPrice`, `memberPrice`, `availability`, "
                + "`Category`.`id` as `categoryId`, `Category`.`name` as `categoryName` "
                + "FROM `Product` INNER JOIN `Category` ON `Category`.`id` = `Product`.`category` "
                + "WHERE `Product`.`id` = @id";
            readCommand.Parameters.AddWithValue("@id", key);

            using var result = readCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce produit");
            }

            return Hydrate(result);
        }

        public Product Update(int key, Product item)
        {
            using var connection = this.Connect();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText
                = "UPDATE `Product` SET `name` = @name, `stock` = @stock, "
                + "`nonMemberPrice` = @nonMemberPrice, `memberPrice` = @memberPrice, "
                + "`availability` = @availability, `category` = @category WHERE `id` = @id";
            updateCommand.Parameters.AddWithValue("@name", item.Name);
            updateCommand.Parameters.AddWithValue("@stock", item.Stock);
            updateCommand.Parameters.AddWithValue("@nonMemberPrice", item.NonMemberPrice);
            updateCommand.Parameters.AddWithValue("@memberPrice", item.MemberPrice);
            updateCommand.Parameters.AddWithValue("@availability", item.Availability);
            updateCommand.Parameters.AddWithValue("@category", item.Category.Id);
            updateCommand.Parameters.AddWithValue("@id", key);

            int affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Ce produit");
            }

            item.Id = key;
            return item;
        }

        public void WithdrawFromStock(int id, int amount)
        {
            using var connection = this.Connect();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText
                = "UPDATE `Product` SET `stock` = `stock` - @amount WHERE `id` = @id";
            updateCommand.Parameters.AddWithValue("@amount", amount);
            updateCommand.Parameters.AddWithValue("@id", id);

            int affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Ce produit");
            }
        }

        public ProductImage ReadImage(int id)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `imageSize`, `image` FROM `ProductImage` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", id);

            using var result = readCommand.ExecuteReader();

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

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT COUNT(*) FROM `ProductImage` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", id);

            long existingImage = (readCommand.ExecuteScalar() as long? ?? 0);

            var updateCommand = connection.CreateCommand();
            if (existingImage == 1)
            {
                updateCommand.CommandText
                    = "UPDATE `ProductImage` SET `imageSize` = @imageSize, `image` = @image WHERE `id` = @id";
            }
            else
            {
                updateCommand.CommandText
                    = "INSERT INTO `ProductImage`(`id`, `imageSize`, `image`) VALUES (@id, @imageSize, @image)";
            }
            updateCommand.Parameters.AddWithValue("@imageSize", image.Bytes.Length);
            updateCommand.Parameters.AddWithValue("@image", image.Bytes);
            updateCommand.Parameters.AddWithValue("@id", id);

            try
            {
                int affectedRows = updateCommand.ExecuteNonQuery();

                if (affectedRows != 1)
                {
                    throw new ItemNotFoundException("Ce produit");
                }
            }
            catch (MySqlException err)
            {
                // violation de clé étrangère
                if (err.ErrorCode == MySqlErrorCode.NoReferencedRow2)
                {
                    throw new ItemNotFoundException("Ce produit");
                }
                else throw;
            }
        }

        public void UnsetImage(int id)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `ProductImage` WHERE `id` = @id";
            command.Parameters.AddWithValue("@id", id);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("L'image de ce produit");
            }
        }
    }
}
