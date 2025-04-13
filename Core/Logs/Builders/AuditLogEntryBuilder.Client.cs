using GalliumPlus.Core.Applications;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IClientEntryBuilder : IGenericEntryBuilder
    {
        AuditLogEntryBuilder NewSecretGenerated(string purpose);
    }

    private class ClientEntryBuilder(Client client, AuditLogEntryBuilder root)
        : GenericEntryBuilder(
            root,
            LoggedAction.ClientAdded, LoggedAction.ClientModified, LoggedAction.ClientDeleted,
            builder =>
            {
                builder.details.Add("id", client.Id);
                builder.details.Add("name", client.Name);
            }
        ), IClientEntryBuilder
    {
        public AuditLogEntryBuilder NewSecretGenerated(string purpose)
        {
            this.Root.action = LoggedAction.ClientNewSecretGenerated;
            this.Root.details.Add("id", client.Id);
            this.Root.details.Add("name", client.Name);
            this.Root.details.Add("purpose", purpose);
            return this.Root;
        }
    }
}