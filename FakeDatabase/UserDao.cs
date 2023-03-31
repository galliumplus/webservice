using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{
    public class UserDao : BaseDao<string, User>, IUserDao
    {
        private RoleDao roles;

        public UserDao(RoleDao roles)
        {
            this.roles = roles;

            this.Create(
                new User(
                    "lomens", "Nicolas RESIN", this.roles.Read(0), "Prof", 20, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    "mf187870", "Matéo FAVARD", this.roles.Read(1), "2A", 1_000_000_000, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    "eb069420", "Evan BEUGNOT", this.roles.Read(2), "1A", double.MaxValue, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
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
