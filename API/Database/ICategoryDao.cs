using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface ICategoryDao
    {
        public void Create(Category product);

        public IEnumerable<Category> ReadAll();

        public Category? ReadOne(int id);

        public void Update(int id, Category category);

        public void Delete(int id);
    }
}
