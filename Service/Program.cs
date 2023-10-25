using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Middleware;
using GalliumPlus.WebApi.Middleware.Authentication;
using GalliumPlus.WebApi.Middleware.Authorization;
using GalliumPlus.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json;
using GalliumPlus.WebApi;
using GalliumPlus.WebApi.Core.Users;
#if FAKE_DB
using GalliumPlus.WebApi.Data.FakeDatabase;
#else
using GalliumPlus.WebApi.Data.MariaDb;
#endif

var builder = WebApplication.CreateBuilder(args);

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

#if FAKE_DB
// ajout en singleton, sinon les données ne sont pas persistées d'une requête à l'autre
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

builder.Services.AddSingleton(
    new DatabaseConnector(
        galliumOptions.MariaDb.Host,
        galliumOptions.MariaDb.UserId,
        galliumOptions.MariaDb.Password,
        galliumOptions.MariaDb.Schema,
        galliumOptions.MariaDb.Port
    )
);
#endif

builder.Services.Configure<JsonOptions>(options =>
{
    // accepte uniquement le format nombre JSON pour les entier et les floats
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    // accepte les virgules en fin de liste / d'objet
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    // garde les noms de propriétés tels quels
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    // sérialise les énumérations sous forme de texte
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// configuration HTTP/HTTPS
builder.WebHost.ConfigureKestrel(opt =>
{
    Action<ListenOptions> httpsConfiguration = options =>
    {
        if (galliumOptions.CertificateFile is string certififcate)
        {
            if (galliumOptions.CertificatePassword is string password)
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

builder.Services.AddServerInfo();

builder.Services
    .AddAuthentication(defaultScheme: "Bearer")
    .AddBearer()
    .AddBasic()
    .AddKeyAndSecret();

var app = builder.Build();

if (galliumOptions.ForceHttps) app.UseHttpsRedirection();
app.UseServerInfo();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

ServerInfo.Current.SetVersion(0, 7, 0, "alpha");
Console.WriteLine(ServerInfo.Current);

app.Run();
