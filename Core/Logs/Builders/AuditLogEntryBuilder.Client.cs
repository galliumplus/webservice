using GalliumPlus.Core.Applications;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IAuditLogClientEntryBuilder
    {
        AuditLogEntryBuilder Added();
        AuditLogEntryBuilder Modified();
        AuditLogEntryBuilder Deleted();
        AuditLogEntryBuilder NewSecretGenerated(string purpose);
    }
    
    private class AuditLogClientEntryBuilder(Client client, AuditLogEntryBuilder root) : IAuditLogClientEntryBuilder
    {
        public AuditLogEntryBuilder Added()
        {
            root.action = LoggedAction.ClientAdded;
            root.details.Add("id", client.Id);
            root.details.Add("name", client.Name);
            return root;
        }
        
        public AuditLogEntryBuilder Modified()
        {
            root.action = LoggedAction.ClientModified;
            root.details.Add("id", client.Id);
            root.details.Add("name", client.Name);
            return root;
        }
        
        public AuditLogEntryBuilder Deleted()
        {
            root.action = LoggedAction.ClientDeleted;
            root.details.Add("id", client.Id);
            root.details.Add("name", client.Name);
            return root;
        }

        public AuditLogEntryBuilder NewSecretGenerated(string purpose)
        {
            root.action = LoggedAction.ClientNewSecretGenerated;
            root.details.Add("id", client.Id);
            root.details.Add("name", client.Name);
            root.details.Add("purpose", purpose);
            return root;
        }
    }
}