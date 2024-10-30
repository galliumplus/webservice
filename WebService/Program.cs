#region Usings
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Quartz;
using System.Text.Json;
using System.Text.Json.Serialization;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Email;
using GalliumPlus.WebService;
using GalliumPlus.WebService.Dto;
using GalliumPlus.WebService.Middleware;
using GalliumPlus.WebService.Middleware.Authentication;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Middleware.ErrorHandling;
using GalliumPlus.WebService.Scheduling;
using GalliumPlus.WebService.Services;

#if FAKE_DB
using GalliumPlus.Data.Fake;
#else
using GalliumPlus.Data.MariaDb;
using GalliumPlus.Data.MariaDb.Implementations;
using FluentMigrator.Runner;
using KiwiQuery;
#endif

#if FAKE_EMAIL
using GalliumPlus.Email.Fake;
#else
using GalliumPlus.Email.MailKit;
#endif

#endregion

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Configuration générale et options

builder.Services
    .AddControllers(options =>
    {
        // Filtre pour les exceptions propres à Gallium
        options.Filters.Add<ExceptionsFilter>();
        // Filtre pour les permissions de Gallium
        options.Filters.Add<PermissionsFilter>();

        // Pour accepter les images
        options.InputFormatters.Add(new AnyBinaryDataInputFormatter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        ExceptionsFilter.ConfigureInvalidModelStateResponseFactory(options);
        options.SuppressMapClientErrors = true;
    });

GalliumOptions galliumOptions = builder.Configuration.GetSection("Gallium").Get<GalliumOptions>() ?? new GalliumOptions();
builder.Services.AddSingleton(galliumOptions);

builder.Services.AddServerInfo();

builder.Services.AddGalliumServices();

#endregion

#region Base de données (Fake & MariaDB)

#if FAKE_DB
// ajout en singleton, sinon les données ne sont pas gardées d'une requête à l'autre
builder.Services.AddSingleton<ICategoryDao, CategoryDao>();
builder.Services.AddSingleton<IClientDao, ClientDao>();
builder.Services.AddSingleton<IHistoryDao, LogsDao>();
builder.Services.AddSingleton<ILogsDao, LogsDao>();
builder.Services.AddSingleton<IProductDao, ProductDao>();
builder.Services.AddSingleton<IRoleDao, RoleDao>();
builder.Services.AddSingleton<ISessionDao, SessionDao>();
builder.Services.AddSingleton<IUserDao, UserDao>();
#else
builder.Services.AddScoped<ICategoryDao, CategoryDao>();
builder.Services.AddScoped<IClientDao, ClientDao>();
builder.Services.AddScoped<IHistoryDao, HistoryDao>();
builder.Services.AddSingleton<ILogsDao, HistoryDao>();
builder.Services.AddScoped<IProductDao, ProductDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<ISessionDao, SessionDao>();
builder.Services.AddScoped<IUserDao, UserDao>();

builder.Services.AddSingleton(new DatabaseConnector(galliumOptions.MariaDb));

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(mrb =>
    {
        mrb.AddMySql8()
            .WithGlobalConnectionString(galliumOptions.MariaDb.ToConnectionString())
            .ScanIn(typeof(MigrationsRunner).Assembly).For.Migrations();
    });
#endif

#endregion

#region Planification (Quartz)

builder.Services.AddQuartz(quartz =>
{
    quartz.AddJobs();
});

builder.Services.AddQuartzHostedService(quartz =>
{
    quartz.WaitForJobsToComplete = true;
});

#endregion

#region Envoi de mail

builder.Services
    .AddSingleton<IEmailTemplateLoader, CachedLocalEmailTemplateLoader>(
        _ => new CachedLocalEmailTemplateLoader(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "templates"))
    )
#if FAKE_EMAIL
    .AddSingleton<IEmailSender, FakeEmailSender>(_ => new FakeEmailSender());
#else
    .AddSingleton<IEmailSender, EmailSender>(_ => new EmailSender(galliumOptions.MailKit));
#endif

#endregion

#region Sérialisation (JSON)

builder.Services.Configure<JsonOptions>(options =>
{
    // accepte uniquement le format nombre JSON pour les entier et les floats
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    // accepte les virgules en fin de liste / d'objet
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    // garde les noms de propriétés tels quels
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    // sérialise les permissions et les portées sous forme numérique
    options.JsonSerializerOptions.Converters.Add(new PermissionsCodeConverter());
    options.JsonSerializerOptions.Converters.Add(new SameSignOnScopesCodeConverter());
    // et les autres énumérations sous forme de texte
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

#endregion

#region HTTP/HTTPS et CORS

builder.WebHost.ConfigureKestrel(opt =>
{
    Action<ListenOptions> httpsConfiguration = options =>
    {
        if (galliumOptions.CertificateFile is { } certificate)
        {
            if (galliumOptions.CertificatePassword is { } password)
            {
                options.UseHttps(certificate, password);
            }
            else
            {
                options.UseHttps(certificate);
            }
        }
        else
        {
            options.UseHttps();
        }
    };

    if (galliumOptions.ListenAnyIp)
    {
        opt.ListenAnyIP(galliumOptions.HttpPort);
        if (!galliumOptions.DisableHttps) opt.ListenAnyIP(galliumOptions.HttpsPort, httpsConfiguration);
    }
    else
    {
        opt.ListenLocalhost(galliumOptions.HttpPort);
        if (!galliumOptions.DisableHttps) opt.ListenLocalhost(galliumOptions.HttpsPort, httpsConfiguration);
    }
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

#endregion

#region Authentification

builder.Services
    .AddAuthentication(defaultScheme: "Bearer")
    .AddBearer()
    .AddBasic()
    .AddKeyAndSecret();

#endregion

WebApplication app = builder.Build();

if (galliumOptions.ForceHttps) app.UseHttpsRedirection();
app.UseServerInfo();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

ServerInfo.Current.SetVersion(1, 1, 0, "beta");
Console.WriteLine(ServerInfo.Current);

#if !FAKE_DB
using (IServiceScope scope = app.Services.CreateScope())
{
    Schema.AlwaysLogTo(scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("KIWIQ"));
    MigrationsRunner.UpdateDatabase(scope.ServiceProvider);
}
#endif

app.Run();
