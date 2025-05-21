using GalliumPlus.Core.Data;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Data.Fake
{
    public class CategoryDao : BaseDaoWithAutoIncrement<Category>, ICategoryDao
    {
        public CategoryDao()
        {
            this.Create(new Category(0, "Boissons", CategoryType.Category));
            this.Create(new Category(0, "Snacks", CategoryType.Category));
            this.Create(new Category(0, "Pablo", CategoryType.Category));
        }

        public override Category Update(int key, Category item)
        {
            CategoryType originalType = this.Read(key).Type;
            return base.Update(key, new Category(item.Id, item.Name, originalType));
        }

        protected override int GetKey(Category item) => item.Id;

        protected override Category SetKey(Category item, int key) => new(key, item.Name, item.Type);
    }
}
