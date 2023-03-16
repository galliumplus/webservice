using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeProductDao : IProductDao
    {
        private List<Product> products;

        public FakeProductDao()
        {
            this.products = new List<Product> {
                new Product {
                    Id = 0,
                    Name = "KitKat",
                    Stock = 20,
                    NonMemberPrice = 0.80,
                    MemberPrice = 0.60,
                    Availability = Availability.AUTO,
                    CategoryId = 0,
                },
                new Product {
                    Id = 1,
                    Name = "Coca-Cola Cherry",
                    Stock = 17,
                    NonMemberPrice = 1.00,
                    MemberPrice = 0.80,
                    Availability = Availability.AUTO,
                    CategoryId = 0,
                },
                new Product {
                    Id = 2,
                    Name = "Pablo",
                    Stock = 20,
                    NonMemberPrice = 999.99,
                    MemberPrice = 500.00,
                    Availability = Availability.ALWAYS,
                    CategoryId = 0,
                },
                new Product {
                    Id = 3,
                    Name = "Kinder Bueno",
                    Stock = 0,
                    NonMemberPrice = 1.00,
                    MemberPrice = 0.80,
                    Availability = Availability.AUTO,
                    CategoryId = 0,
                },
            };
        }

        public void Create(Product product)
        {
            lock (products)
            {
                int id = products.Count;
                product.Id = id;
                this.products.Add(product);
            }
        }

        public IEnumerable<Product> ReadAll()
        {
            return this.products;
        }

        public Product ReadOne(int id)
        {
            return this.products[id];
        }

        public void Update(int id, Product update)
        {
            lock (products)
            {
                if (update.Name != null) this.products[id].Name = update.Name;
            }
        }

        public void Delete(int id)
        {
            lock (products)
            {
                this.products.RemoveAt(id);
            }
        }

        public IEnumerable<Product> ReadAvailable()
        {
            return this.products.FindAll(product => product.Available);
        }
    }
}
