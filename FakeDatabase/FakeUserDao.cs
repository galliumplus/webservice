using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeUserDao : FakeDao<string, User>, IUserDao
    {
        public FakeUserDao()
        {
            this.Create(
                new User
                {
                    Id = "mf187870",
                    Name = "Matéo FAVARD",
                    Role = 0,
                    Year = "2A",
                    Deposit = 1_000_000_000,
                    RequireValidationForPayments = false,
                }
            );
            this.Create(
                new User
                {
                    Id = "eb069420",
                    Name = "Evan BEUGNOT",
                    Role = 1,
                    Year = "1A",
                    Deposit = Double.MaxValue,
                    RequireValidationForPayments = false,
                }
            );
        }

        public void UpdateDeposit(string id, double deposit)
        {
            User user = this.ReadOne(id);
            user.Deposit = deposit;
            this.Update(id, user);
        }
    }
}
