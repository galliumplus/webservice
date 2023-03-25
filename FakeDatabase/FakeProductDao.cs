using GalliumPlus.WebApi.Models;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{
    public class FakeProductDao : IProductDao
    {
        private List<Product> products;

        public FakeProductDao()
        {
            this.products = new List<Product> {
                new Product (
                    0,
                    "KitKat",
                    20,
                    0.80,
                    0.60,
                    Availability.AUTO,
                    0
                ),
                new Product (
                    1,
                    "Coca-Cola Cherry",
                    17,
                    1.00,
                    0.80,
                    Availability.AUTO,
                    0
                ),
                new Product (
                    2,
                    "Pablo",
                    20,
                    999.99,
                    500.00,
                    Availability.ALWAYS,
                    0
                ),
                new Product (
                    3,
                    "Kinder Bueno",
                    0,
                    1.00,
                    0.80,
                    Availability.AUTO,
                    0
                ),
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

        public IEnumerable<Product> Read()
        {
            return this.products;
        }

        public Product Read(int id)
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
