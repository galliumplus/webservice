namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IGenericEntryBuilder
    {
        AuditLogEntryBuilder Added();
        AuditLogEntryBuilder Modified();
        AuditLogEntryBuilder Deleted();
    }

    private abstract class GenericEntryBuilder(
        AuditLogEntryBuilder root,
        LoggedAction addedAction,
        LoggedAction modifiedAction,
        LoggedAction deletedAction
    ) : IGenericEntryBuilder
    {
        protected AuditLogEntryBuilder Root => root;

        protected abstract void AddDetails();
        
        public AuditLogEntryBuilder Added()
        {
            this.AddDetails();
            this.Root.action = addedAction;
            return this.Root;
        }

        public AuditLogEntryBuilder Modified()
        {
            this.AddDetails();
            this.Root.action = modifiedAction;
            return this.Root;
        }
        
        public AuditLogEntryBuilder Deleted()
        {
            this.AddDetails();
            this.Root.action = deletedAction;
            return this.Root;
        }
    }
}