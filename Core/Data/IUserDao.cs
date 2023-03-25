using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface IUserDao : IBasicDao<string, User>
    {
        public void UpdateDeposit(string id, double deposit);
    }
}
