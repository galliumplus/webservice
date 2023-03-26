using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{
    public class UserDao : BaseDao<string, User>, IUserDao
    {
        public UserDao()
        {
            this.Create(
                new User("lomens", "Nicolas RESIN", 1, "Prof", 20)
            );
            this.Create(
                new User("mf187870", "Matéo FAVARD", 0, "2A", 1_000_000_000)
            );
            this.Create(
                new User("eb069420", "Evan BEUGNOT", 1, "1A", double.MaxValue)
            );
        }

        public void UpdateDeposit(string id, double deposit)
        {
            User user = this.Read(id);
            user.Deposit = deposit;
            Update(id, user);
        }

        protected override string GetKey(User item) => item.Id;
    }
}
