using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeProductDao : FakeDaoWithAutoIncrement<Product>, IProductDao
    {
        public FakeProductDao()
        {
            this.Create(
                new Product
                {
                    Name = "KitKat",
                    Stock = 20,
                    NonMemberPrice = 0.80,
                    MemberPrice = 0.60,
                    Availability = Availability.AUTO,
                    Category = 1,
                }
            );
            this.Create(
                new Product
                {
                    Name = "Coca-Cola Cherry",
                    Stock = 17,
                    NonMemberPrice = 1.00,
                    MemberPrice = 0.80,
                    Availability = Availability.AUTO,
                    Category = 0,
                }
            );
            this.Create(
                new Product
                {
                    Name = "Pablo",
                    Stock = 1,
                    NonMemberPrice = 999.99,
                    MemberPrice = 500.00,
                    Availability = Availability.ALWAYS,
                    Category = 2,
                }
            );
            this.Create(
                new Product
                {
                    Name = "Kinder Bueno",
                    Stock = 0,
                    NonMemberPrice = 1.00,
                    MemberPrice = 0.80,
                    Availability = Availability.AUTO,
                    Category = 1,
                }
            );
            this.Create(
                new Product
                {
                    Name = "Chocolat chaud",
                    Stock = 11,
                    NonMemberPrice = 0.70,
                    MemberPrice = 0.50,
                    Availability = Availability.AUTO,
                    Category = 0,
                }
            );
            this.Create(
                new Product
                {
                    Name = "Madeleine",
                    Stock = 24,
                    NonMemberPrice = 1.00,
                    MemberPrice = 0.80,
                    Availability = Availability.AUTO,
                    Category = 1,
                }
            );
        }

        private static Func<Product, bool> ToPredicate(ProductCriteria criteria)
        {
            return product => (!criteria.AvailableOnly || product.Available)
                           && (criteria.Category == null || product.Category == criteria.Category);
        }

        public IEnumerable<Product> FindAll(ProductCriteria criteria)
        {
            return this.Items.Values.Where(ToPredicate(criteria));
        }

        protected override void SetAutoKey(Product item)
        {
            item.Id = this.NextInsertKey;
        }
    }
}
