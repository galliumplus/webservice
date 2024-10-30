using GalliumPlus.Core.Data;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Data.Fake
{
    public class CategoryDao : BaseDaoWithAutoIncrement<Category>, ICategoryDao
    {
        public CategoryDao()
        {
            this.Create(new Category(0, "Boissons"));

            this.Create(new Category(0, "Snacks"));

            this.Create(new Category(0, "Pablo"));
        }

        protected override int GetKey(Category item) => item.Id;

        protected override void SetKey(ref Category item, int key) => item = item.WithId(key);
    }
}
