using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Dto
{
    public class LoggedIn
    {
        public string Token { get; }
        public DateTime Expiration { get; }
        public UserDetails User { get; }

        public LoggedIn(string token, DateTime expiration, UserDetails user)
        {
            this.Token = token;
            this.Expiration = expiration;
            this.User = user;
        }

        public class Mapper : Mapper<Session, LoggedIn, ISessionDao>
        {
            private UserDetails.Mapper userMapper = new();

            public override LoggedIn FromModel(Session model)
            {
                return new(
                    model.Token,
                    model.Expiration,
                    this.userMapper.FromModel(model.User)
                );
            }

            public override Session ToModel(LoggedIn dto, ISessionDao dao)
            {
                throw new InvalidOperationException("LoggedIn objects are read-only");
            }
        }
    }
}
 