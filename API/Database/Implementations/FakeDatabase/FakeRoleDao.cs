using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeRoleDao : IRoleDao
    {
        public void Create(Role item)
        {
            throw new NotImplementedException();
        }

        public void Delete(int key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Role? ReadOne(int key)
        {
            throw new NotImplementedException();
        }

        public void Update(int key, Role item)
        {
            throw new NotImplementedException();
        }
    }
}
