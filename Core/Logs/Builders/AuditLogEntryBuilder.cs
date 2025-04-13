using System.Text.Json;
using System.Text.Json.Nodes;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    private LoggedAction? action;
    private int? clientId;
    private int? userId;
    private readonly JsonObject details = new();

    public AuditLog Build()
    {
        if (this.action == null) throw new InvalidOperationException("Aucun type d'action défini.");
        if (this.clientId == null) throw new InvalidOperationException("Aucun acteur défini.");

        return new AuditLog(
            id: 0,
            action: this.action.Value,
            time: DateTime.UtcNow,
            client: this.clientId.Value,
            user: this.userId,
            details: JsonSerializer.Serialize(this.details)
        );
    }
    
    public AuditLogEntryBuilder By(Client app, User? user = null)
    {
        this.clientId = app.Id;
        this.userId = user?.Iuid;
        return this;
    }

    public IGenericEntryBuilder Category(Category category)
        => new GenericEntryBuilder(
            this,
            LoggedAction.CategoryAdded, LoggedAction.CategoryModified, LoggedAction.CategoryDeleted,
            root => {
                root.details.Add("id", category.Id);
                root.details.Add("name", category.Name);
            }
        );

    public IClientEntryBuilder Client(Client client) => new ClientEntryBuilder(client, this);

    public IUserEntryBuilder User(User user) => new UserEntryBuilder(user, this);
}