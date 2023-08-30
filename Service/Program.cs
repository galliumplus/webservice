using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Middleware;
using GalliumPlus.WebApi.Middleware.Authentication;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
#if FAKE_DB
using GalliumPlus.WebApi.Data.FakeDatabase;
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

#if FAKE_DB
// ajout en singleton, sinon les données ne sont pas persistées d'une requête à l'autre
builder.Services.AddSingleton<ICategoryDao, CategoryDao>();
builder.Services.AddSingleton<IProductDao, ProductDao>();
builder.Services.AddSingleton<IRoleDao, RoleDao>();
builder.Services.AddSingleton<ISessionDao, SessionDao>();
builder.Services.AddSingleton<IUserDao, UserDao>();
#endif

builder.Services.Configure<JsonOptions>(options =>
{
    // accepte uniquement le format nombre JSON pour les entier et les floats
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    // accepte les virgules en fin de liste / d'objet
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    // garde les noms de propriétés tels quels
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    // sérialise les énumérations sous forme de texte
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// configuration HTTP/HTTPS
builder.WebHost.ConfigureKestrel(opt =>
{
    if (!Int32.TryParse(Environment.GetEnvironmentVariable("GALLIUM_HTTP"), out int httpPort))
    {
        // default HTTP port
        httpPort = 5080;
    }

    if (!Int32.TryParse(Environment.GetEnvironmentVariable("GALLIUM_HTTPS"), out int httpsPort))
    {
        // default HTTPS port
        httpsPort = 5443;
    }

    Action<ListenOptions> httpsConfiguration = options =>
    {
        if (Environment.GetEnvironmentVariable("GALLIUM_CERTIFICATE_FILE") is string certififcate)
        {
            if (Environment.GetEnvironmentVariable("GALLIUM_CERTIFICATE_PASSWORD") is string password)
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

    if (Environment.GetEnvironmentVariable("GALLIUM_LISTEN_ANY_IP") is string)
    {
        opt.ListenAnyIP(httpPort);
        opt.ListenAnyIP(httpsPort, httpsConfiguration);
    }
    else
    {
        opt.ListenLocalhost(httpPort);
        opt.ListenLocalhost(httpsPort, httpsConfiguration);
    }
});

builder.Services.AddServerInfo();

builder.Services
    .AddAuthentication(defaultScheme: "Bearer")
    .AddBearer()
    .AddBasic();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseServerInfo();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

ServerInfo.Current.SetVersion(0, 3, 2, "alpha");
Console.WriteLine(ServerInfo.Current);

app.Run();
