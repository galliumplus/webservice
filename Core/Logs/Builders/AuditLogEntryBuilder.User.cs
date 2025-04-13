using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IUserEntryBuilder : IGenericEntryBuilder
    {
        AuditLogEntryBuilder DepositClosed();
    }

    private class UserEntryBuilder(User user, AuditLogEntryBuilder root)
        : GenericEntryBuilder(
            root,
            LoggedAction.UserAdded, LoggedAction.UserModified, LoggedAction.UserDeleted,
            builder =>
            {
                builder.details.Add("id", user.Id);
            }
        ), IUserEntryBuilder
    {
        public AuditLogEntryBuilder DepositClosed()
        {
            this.Root.action = LoggedAction.UserDepositClosed;
            this.Root.details.Add("id", user.Id);
            return this.Root;
        }
    }
}