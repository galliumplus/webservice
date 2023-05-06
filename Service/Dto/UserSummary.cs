using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto
{
    public class UserSummary
    {
        public string Id { get; }
        public string Name { get; }
        public int Role { get; }
        public string Year { get; }
        public double Deposit { get; }
        public bool FormerMember { get; }

        [JsonConstructor]
        public UserSummary(string id, string name, int role, string year, double deposit, bool formerMember)
        {
            Id = id;
            Name = name;
            Role = role;
            Year = year;
            Deposit = deposit;
            FormerMember = formerMember;
        }

        public class Mapper : IMapper<User, UserSummary>
        {
            public override UserSummary FromModel(User user)
            {
                return new UserSummary(
                    user.Id,
                    user.Name,
                    user.Role.Id,
                    user.Year,
                    user.Deposit,
                    user.FormerMember
                );
            }

            public override User ToModel(UserSummary summary, IMasterDao dao)
            {
                return new User(
                    summary.Id,
                    summary.Name,
                    dao.Roles.Read(summary.Role),
                    summary.Year,
                    summary.Deposit,
                    summary.FormerMember
                );
            }
        }
    }
}
