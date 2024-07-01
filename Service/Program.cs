#region Usings

using GalliumPlus.WebApi;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Middleware;
using GalliumPlus.WebApi.Middleware.Authentication;
using GalliumPlus.WebApi.Middleware.Authorization;
using GalliumPlus.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Quartz;
using System.Text.Json;
using System.Text.Json.Serialization;
using GalliumPlus.WebApi.Scheduling;
using GalliumPlus.WebApi.Core.Email;
#if FAKE_DB && FAKE_EMAIL
using GalliumPlus.WebApi.Data.FakeDatabase;
using GalliumPlus.WebApi.Email.FakeEmailService;
#else
using GalliumPlus.WebApi.Data.MariaDb;
using GalliumPlus.WebApi.Email.MailKit;
#endif 

#endregion

var builder = WebApplication.CreateBuilder(args);

#region Configuration g?n?rale et options

builder.Services
    .AddControllers(options =>
    {
        // Filtre pour les exceptions propres ? Gallium
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

#endregion

#region Base de donn?es (Fake & MariaDB)

#if FAKE_DB
// ajout en singleton, sinon les donn?es ne sont pas persist?es d'une requ?te ? l'autre
builder.Services.AddSingleton<ICategoryDao, CategoryDao>();
builder.Services.AddSingleton<IClientDao, ClientDao>();
builder.Services.AddSingleton<IHistoryDao, HistoryDao>();
builder.Services.AddSingleton<IProductDao, ProductDao>();
builder.Services.AddSingleton<IRoleDao, RoleDao>();
builder.Services.AddSingleton<ISessionDao, SessionDao>();
builder.Services.AddSingleton<IUserDao, UserDao>();
#else
builder.Services.AddScoped<ICategoryDao, CategoryDao>();
builder.Services.AddScoped<IClientDao, ClientDao>();
builder.Services.AddScoped<IHistoryDao, HistoryDao>();
builder.Services.AddScoped<IProductDao, ProductDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<ISessionDao, SessionDao>();
builder.Services.AddScoped<IUserDao, UserDao>();

builder.Services.AddSingleton(new DatabaseConnector(galliumOptions.MariaDb));
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
        services => new CachedLocalEmailTemplateLoader(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "templates"))
    )
#if FAKE_EMAIL
    .AddSingleton<IEmailSender, FakeEmailSender>(services => new FakeEmailSender());
#else
    .AddSingleton<IEmailSender, EmailSender>(services => new EmailSender(galliumOptions.MailKit));
#endif

#endregion

#region S?rialisation (JSON)

builder.Services.Configure<JsonOptions>(options =>
{
    // accepte uniquement le format nombre JSON pour les entier et les floats
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    // accepte les virgules en fin de liste / d'objet
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    // garde les noms de propri?t?s tels quels
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    // s?rialise les ?num?rations sous forme de texte
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

#endregion

#region HTTP/HTTPS et CORS

builder.WebHost.ConfigureKestrel(opt =>
{
    Action<ListenOptions> httpsConfiguration = options =>
    {
        if (galliumOptions.CertificateFile is { } certififcate)
        {
            if (galliumOptions.CertificatePassword is { } password)
            {
                options.UseHttps(certififcate, password);
            }
            else
            {
                options.UseHttps(certififcate);
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

var app = builder.Build();

if (galliumOptions.ForceHttps) app.UseHttpsRedirection();
app.UseServerInfo();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

ServerInfo.Current.SetVersion(1, 0, 3, "beta");
Console.WriteLine(ServerInfo.Current);

app.Run();
