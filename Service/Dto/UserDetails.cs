using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto
{
    public class UserDetails
    {
        public string Id { get; }
        public string Name { get; }
        public RoleDetails Role { get; }
        public string Year { get; }
        public double Deposit { get; }
        public bool IsMember { get; }

        public UserDetails(string id, string name, RoleDetails role, string year, double deposit, bool isMember)
        {
            Id = id;
            Name = name;
            Role = role;
            Year = year;
            Deposit = deposit;
            IsMember = isMember;
        }

        public class Mapper : Mapper<User, UserDetails, IUserDao>
        {
            private RoleDetails.Mapper roleMapper = new();

            public override UserDetails FromModel(User user)
            {
                return new UserDetails(
                    user.Id,
                    user.Name,
                    roleMapper.FromModel(user.Role),
                    user.Year,
                    user.Deposit,
                    user.IsMember
                );
            }

            public override User ToModel(UserDetails details, IUserDao dao)
            {
                return new User(
                    details.Id,
                    details.Name,
                    roleMapper.ToModel(details.Role, dao.Roles),
                    details.Year,
                    details.Deposit,
                    details.IsMember
                );
            }
        }
    }
}
