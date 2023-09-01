using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace GalliumPlus.WebApi.Dto
{
    public class UserSummary
    {
        [Required] public string Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int? Role { get; set; }
        [Required] public string Year { get; set; }
        [Required] public double? Deposit { get; set; }
        [Required] public bool? IsMember { get; set; }

        public UserSummary()
        {
            this.Id = String.Empty;
            this.Name = String.Empty;
            this.Role = null;
            this.Year = String.Empty;
            this.Deposit = null;
            this.IsMember = null;
        }

        public class Mapper : Mapper<User, UserSummary, IUserDao>
        {
            public override UserSummary FromModel(User user)
            {
                return new UserSummary
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role.Id,
                    Year = user.Year,
                    Deposit = user.Deposit,
                    IsMember = user.IsMember
                };
            }

            public override User ToModel(UserSummary summary, IUserDao dao)
            {
                Role role;
                try
                {
                    role = dao.Roles.Read(summary.Role!.Value);
                }
                catch (ItemNotFoundException)
                {
                    throw new InvalidItemException("Le rôle associé n'existe pas");
                }

                return new User(
                    summary.Id!,
                    summary.Name!,
                    role,
                    summary.Year!,
                    summary.Deposit!.Value,
                    summary.IsMember!.Value
                );
            }
        }
    }
}
