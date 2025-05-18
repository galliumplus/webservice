using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    private class RoleEntryBuilder(AuditLogEntryBuilder root, Role role)
        : GenericEntryBuilder(
            root,
            LoggedAction.RoleAdded,
            LoggedAction.RoleModified,
            LoggedAction.RoleDeleted
        )
    {
        protected override void AddDetails()
        {
            this.Root.details.Add("id", role.Id);
            this.Root.details.Add("name", role.Name);
        }
    }
}