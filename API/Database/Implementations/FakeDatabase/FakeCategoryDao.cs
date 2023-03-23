using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeCategoryDao : FakeDaoWithAutoIncrement<Category>, ICategoryDao
    {
        public FakeCategoryDao()
        {
            this.Create(new Category { Name = "Boisson" });
            this.Create(new Category { Name = "Snack" });
            this.Create(new Category { Name = "Pablo" });
        }

        protected override void SetAutoKey(Category item)
        {
            item.Id = this.NextInsertKey;
        }
    }
}
