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

    public AuditLogEntryBuilder By(Session session)
    {
        this.clientId = session.Client.Id;
        this.userId = session.User?.Iuid;
        return this;
    }

    public IGenericEntryBuilder Category(Category category) => new CategoryEntryBuilder(this, category);

    public IClientEntryBuilder Client(Client client) => new ClientEntryBuilder(this, client);

    public IItemEntryBuilder Item(Product item) => new ItemEntryBuilder(this, item);
    
    public IGenericEntryBuilder PriceList(PriceList priceList) => new PriceListEntryBuilder(this, priceList);

    public IGenericEntryBuilder Role(Role role) => new RoleEntryBuilder(this, role);
    
    public IUserEntryBuilder User(User user) => new UserEntryBuilder(this, user);
    
    public AuditLogEntryBuilder UserConnection()
    {
        this.action = LoggedAction.UserLoggedIn;
        return this;
    }
    
    public AuditLogEntryBuilder ApplicationConnection()
    {
        this.action = LoggedAction.ApplicationConnected;
        return this;
    }
    
    public AuditLogEntryBuilder SsoConnectionTo(Client app, string redirectUrl)
    {
        this.action = LoggedAction.SsoUserLoggedIn;
        this.details.Add("app", app.Id);
        this.details.Add("redirectUrl", redirectUrl);
        return this;
    }
}