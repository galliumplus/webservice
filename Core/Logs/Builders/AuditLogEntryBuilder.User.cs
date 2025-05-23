using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IUserEntryBuilder : IGenericEntryBuilder
    {
        AuditLogEntryBuilder DepositClosed();
    }

    private class UserEntryBuilder(AuditLogEntryBuilder root, User user)
        : GenericEntryBuilder(
            root,
            LoggedAction.UserAdded,
            LoggedAction.UserModified,
            LoggedAction.UserDeleted
        ), IUserEntryBuilder
    {
        protected override void AddDetails()
        {
            this.Root.details.Add("id", user.Id);
        }

        public AuditLogEntryBuilder DepositClosed()
        {
            this.Root.action = LoggedAction.UserDepositClosed;
            this.Root.details.Add("id", user.Id);
            return this.Root;
        }
    }
}