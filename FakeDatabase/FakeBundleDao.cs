using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using System.Runtime.CompilerServices;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeBundleDao : FakeDaoWithAutoIncrement<Bundle>, IBundleDao
    {
        private FakeMasterDao master;

        public FakeBundleDao(FakeMasterDao master)
        {
            this.master = master;

            this.Create(
                new Bundle {
                    Name = "Chocolat Matin",
                    NonMemberPrice = 0.70,
                    MemberPrice = 0.50,
                    Availability = Availability.AUTO,
                    Products = new List<int> {4, 5},
                }
            );
        }

        private Func<Bundle, bool> ToPredicate(BundleCriteria criteria)
        {
            return bundle => !criteria.AvailableOnly || bundle.Available(this.master);
        }

        public IEnumerable<Bundle> FindAll(BundleCriteria criteria)
        {
            return this.Items.Values.Where(ToPredicate(criteria));
        }

        protected override void SetAutoKey(Bundle item)
        {
            item.Id = this.NextInsertKey;
        }

        protected override bool CheckConstraints(Bundle item)
        {
            foreach (int productId in item.Products)
            {
                if (this.master.Products.ReadOne(productId) == null) return false;
            }

            return true;
        }
    }
}
