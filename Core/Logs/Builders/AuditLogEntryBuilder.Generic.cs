namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IGenericEntryBuilder
    {
        AuditLogEntryBuilder Added();
        AuditLogEntryBuilder Modified();
        AuditLogEntryBuilder Deleted();
    }

    private class GenericEntryBuilder(
        AuditLogEntryBuilder root,
        LoggedAction addedAction,
        LoggedAction modifiedAction,
        LoggedAction deletedAction,
        Action<AuditLogEntryBuilder> detailsConfig
    ) : IGenericEntryBuilder
    {
        protected AuditLogEntryBuilder Root => root;
        
        public AuditLogEntryBuilder Added()
        {
            this.Root.action = addedAction;
            detailsConfig.Invoke(this.Root);
            return this.Root;
        }

        public AuditLogEntryBuilder Modified()
        {
            this.Root.action = modifiedAction;
            detailsConfig.Invoke(this.Root);
            return this.Root;
        }
        
        public AuditLogEntryBuilder Deleted()
        {
            this.Root.action = deletedAction;
            detailsConfig.Invoke(this.Root);
            return this.Root;
        }
    }
}