using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class UserDao : BaseDao<string, User>, IUserDao
    {
        private IRoleDao roles;

        public IRoleDao Roles => roles;

        public UserDao(IRoleDao roles)
        {
            this.roles = roles;

            this.Create(
                new User(
                    "lomens",
                    new UserIdentity("Nicolas", "RESIN", "nicolas.resin@iut-dijon.u-bourgogne.fr", "PROF"),
                    this.roles.Read(1), 20, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    "mf187870", 
                    new UserIdentity("Matéo", "FAVARD", "mateo.favard@iut-dijon.u-bourgogne.fr", "3A"),
                    this.roles.Read(2), 1_000_000_000, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    "eb069420",
                    new UserIdentity("Evan", "BEUGNOT", "evan.beugnot@iut-dijon.u-bourgogne.fr", "2A"),
                    this.roles.Read(3), 1_000_000_000, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
        }

        override public User Update(string key, User item)
        {
            try
            {
                if (GetKey(item) == key)
                {
                    if (!this.Items.ContainsKey(key)) throw new ItemNotFoundException();
                    this.Items[key] = item;
                    return item;
                }
                else
                {
                    if (this.Items.ContainsKey(GetKey(item))) throw new DuplicateItemException();
                    this.Items.Remove(key);
                    return this.Create(item);

                }
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        public decimal? ReadDeposit(string id)
        {
            return this.Read(id).Deposit;
        }

        public void AddToDeposit(string id, decimal money)
        {
            User user = this.Read(id);
            if (money < 0) throw new InvalidItemException("L'acompte ne peut pas être négatif.");
            user.Deposit += money;
            Update(id, user);
        }

        protected override string GetKey(User item) => item.Id;

        protected override void SetKey(ref User item, string key)
        {
            throw new InvalidOperationException("Can't set the user ID automatically");
        }

        protected override bool CheckConstraints(User item)
        {
            return item.Deposit > 0;
        }
    }
}
