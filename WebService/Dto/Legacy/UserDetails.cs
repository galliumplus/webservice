using GalliumPlus.Core.Users;

namespace GalliumPlus.WebService.Dto
{
    public class UserDetails
    {
        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public RoleDetails Role { get; }
        public string Year { get; }
        public decimal? Deposit { get; }
        public bool IsMember { get; }

        public UserDetails(string id, string firstName, string lastName, string email, RoleDetails role, string year, decimal? deposit, bool isMember)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Role = role;
            this.Year = year;
            this.Deposit = deposit;
            this.IsMember = isMember;
        }

        public class Mapper : Mapper<User, UserDetails>
        {
            private RoleDetails.Mapper roleMapper = new();

            public override UserDetails FromModel(User user)
            {
                return new UserDetails(
                    user.Id,
                    user.Identity.FirstName,
                    user.Identity.LastName,
                    user.Identity.Email,
                    this.roleMapper.FromModel(user.Role),
                    user.Identity.Year,
                    user.Deposit,
                    user.IsMember
                );
            }

            public override User ToModel(UserDetails details)
            {
                return new User(
                    -1,
                    details.Id,
                    new UserIdentity(
                        details.FirstName,
                        details.LastName,
                        details.Email,
                        details.Year
                    ),
                    this.roleMapper.ToModel(details.Role),
                    details.Deposit,
                    details.IsMember
                );
            }
        }
    }
}
