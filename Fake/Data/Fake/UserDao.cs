using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Data.Fake
{
    public class UserDao : BaseDao<string, User>, IUserDao
    {
        private IRoleDao roles;

        private class PrtDao : BaseDao<string, PasswordResetToken>
        {
            public PrtDao()
            {
                var codeAsPwd = PasswordInformation.FromPassword("secret-code");

                this.Create(
                    new PasswordResetToken(
                        "test-prt-1",
                        new OneTimeSecret(codeAsPwd.Hash, codeAsPwd.Salt),
                        new DateTime(2099, 01, 01),
                        "mf187870"
                    )
                );
            }

            protected override string GetKey(PasswordResetToken item) => item.Token;

            protected override PasswordResetToken SetKey(PasswordResetToken item, string key) => throw new NotImplementedException(); // tkt mon pote
        }
        private PrtDao prtDao;

        public IRoleDao Roles => this.roles;

        public UserDao(IRoleDao roles)
        {
            this.roles = roles;
            this.prtDao = new();

            this.Create(
                new User(
                    1,
                    "lomens",
                    new UserIdentity("Nicolas", "RESIN", "nicolas.resin@iut-dijon.u-bourgogne.fr", "PROF"),
                    this.roles.Read(1), 20, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    2,
                    "mf187870", 
                    new UserIdentity("Matéo", "FAVARD", "mateo.favard@iut-dijon.u-bourgogne.fr", "3A"),
                    this.roles.Read(2), 1_000_000_000, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
            this.Create(
                new User(
                    3,
                    "eb069420",
                    new UserIdentity("Evan", "BEUGNOT", "evan.beugnot@iut-dijon.u-bourgogne.fr", "2A"),
                    this.roles.Read(3), 1_000_000_000, false,
                    PasswordInformation.FromPassword("motdepasse")
                )
            );
        }
        
        private User Update(string key, User item, bool forceDepositModification)
        {
            try
            {
                User old = this.Items[key];
                item.Password = old.Password;
                if (!forceDepositModification) item.Deposit = old.Deposit;
                if (this.GetKey(item) == key)
                {
                    if (!this.Items.ContainsKey(key)) throw new ItemNotFoundException();
                    this.Items[key] = item;
                    return item;
                }
                else
                {
                    if (this.Items.ContainsKey(this.GetKey(item))) throw new DuplicateItemException();
                    this.Items.Remove(key);
                    return this.Create(item);

                }
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        public override User Update(string key, User item) => this.Update(key, item, false);

        public void UpdateForcingDepositModification(string key, User item) => this.Update(key, item, true);

        public decimal? ReadDeposit(string id)
        {
            return this.Read(id).Deposit;
        }

        public void UpdateDeposit(string id, decimal? money)
        {
            User user = this.Read(id);
            user.Deposit = money;
            this.Update(id, user);
        }

        protected override string GetKey(User item) => item.Id;

        protected override User SetKey(User item, string key)
        {
            throw new InvalidOperationException("Can't set the user ID automatically");
        }

        protected override bool CheckConstraints(User item)
        {
            return item.Deposit == null || item.Deposit >= 0;
        }

        public void ChangePassword(string id, PasswordInformation newPassword)
        {
            User user = this.Read(id);
            user.Password = newPassword;
            this.Update(id, user);
        }

        public void CreatePasswordResetToken(PasswordResetToken token)
        {
            this.prtDao.Create(token);
        }

        public PasswordResetToken ReadPasswordResetToken(string token)
        {
            return this.prtDao.Read(token);
        }

        public void DeletePasswordResetToken(string token)
        {
            this.prtDao.Delete(token);
        }
    }
}