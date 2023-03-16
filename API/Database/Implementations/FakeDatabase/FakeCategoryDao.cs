using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeCategoryDao : ICategoryDao
    {
        private List<Category> categories;

        public FakeCategoryDao()
        {
            this.categories = new List<Category> { 
                new Category { Id = 0, Name = "Boisson" },
                new Category { Id = 1, Name = "Snack" },
                new Category { Id = 2, Name = "Pablo" },
            };
        }

        public void Create(Category category)
        {
            lock (categories)
            {
                int id = this.categories.Count;
                category.Id = id;
                this.categories.Add(category);
            }
        }

        public IEnumerable<Category> ReadAll()
        {
            return this.categories;
        }

        public Category? ReadOne(int id)
        {
            try
            {
                return this.categories[id];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public void Update(int id, Category category)
        {
            lock (categories)
            {
                try
                {
                    category.Id = id;
                    this.categories[id] = category;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ItemNotFoundException();
                }
            }
        }

        public void Delete(int id)
        {
            lock (categories)
            {
                try
                {
                    this.categories.RemoveAt(id);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ItemNotFoundException();
                }
            }
        }
    }
}
