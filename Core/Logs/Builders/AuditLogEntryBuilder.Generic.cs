using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IAuditLogGenericEntryBuilder
    {
        AuditLogEntryBuilder Added();
        AuditLogEntryBuilder Modified();
        AuditLogEntryBuilder Deleted();
    }

    private class AuditLogGenericEntryBuilder(
        AuditLogEntryBuilder root,
        LoggedAction addedAction,
        LoggedAction modifiedAction,
        LoggedAction deletedAction,
        Action<AuditLogEntryBuilder> detailsConfig
    ) : IAuditLogGenericEntryBuilder
    {
        public AuditLogEntryBuilder Added()
        {
            root.action = addedAction;
            detailsConfig.Invoke(root);
            return root;
        }

        public AuditLogEntryBuilder Modified()
        {
            root.action = modifiedAction;
            detailsConfig.Invoke(root);
            return root;
        }
        
        public AuditLogEntryBuilder Deleted()
        {
            root.action = deletedAction;
            detailsConfig.Invoke(root);
            return root;
        }
    }
}