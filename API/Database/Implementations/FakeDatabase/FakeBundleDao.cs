using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using System.Runtime.CompilerServices;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeBundleDao : IBundleDao
    {
        private List<Bundle> bundles;
        private FakeDao master;

        public FakeBundleDao(FakeDao master)
        {
            this.master = master;

            this.bundles = new List<Bundle> {
                new Bundle {
                    Id = 0,
                    Name = "Chocolat Matin",
                    NonMemberPrice = 0.70,
                    MemberPrice = 0.50,
                    Availability = Availability.AUTO,
                    Products = new List<int> {4, 5},
                },
            };
        }

        public void Create(Bundle bundle)
        {
            lock (bundles)
            {
                int id = this.bundles.Count;
                bundle.Id = id;
                this.bundles.Add(bundle);
            }
        }

        public void Delete(int id)
        {
            lock (bundles)
            {
                try
                {
                    this.bundles.RemoveAt(id);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ItemNotFoundException();
                }
            }
        }

        private Predicate<Bundle> ToPredicate(BundleCriteria criteria)
        {
            return bundle => !criteria.AvailableOnly || bundle.Available(this.master);
        }

        public IEnumerable<Bundle> FindAll(BundleCriteria criteria)
        {
            return this.bundles.FindAll(ToPredicate(criteria));
        }

        public IEnumerable<Bundle> ReadAll()
        {
            return this.bundles;
        }

        public Bundle ReadOne(int id)
        {
            try
            {
                return this.bundles[id];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ItemNotFoundException();
            }
        }

        public void Update(int id, Bundle bundle)
        {
            lock (bundles)
            {
                try
                {
                    bundle.Id = id;
                    this.bundles[id] = bundle;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ItemNotFoundException();
                }
            }
        }
    }
}