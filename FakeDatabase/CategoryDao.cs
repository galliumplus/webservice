using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Data.FakeDatabase
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

        protected override void SetKey(Category item, int key) => item.Id = key;
    }
}
