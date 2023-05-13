using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Middleware;
using GalliumPlus.WebApi.Middleware.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
#if FAKE_DB
using GalliumPlus.WebApi.Data.FakeDatabase;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        // Ajoute un filtre pour les exceptions propres à gallium
        options.Filters.Add<ExceptionHandler>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        ExceptionHandler.ConfigureInvalidModelStateResponseFactory(options);
        options.SuppressMapClientErrors = true;
    });

#if FAKE_DB
builder.Services.AddSingleton<IRoleDao, RoleDao>();
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
    int httpPort;
    if (!Int32.TryParse(Environment.GetEnvironmentVariable("GALLIUM_HTTP"), out httpPort))
    {
        httpPort = 5080;
    }

    int httpsPort;
    if (!Int32.TryParse(Environment.GetEnvironmentVariable("GALLIUM_HTTPS"), out httpsPort))
    {
        httpsPort = 5443;
    }

    opt.ListenAnyIP(httpPort);
    opt.ListenAnyIP(httpsPort, opt =>
    {
        if (Environment.GetEnvironmentVariable("GALLIUM_CERTIFICATE_FILE") is string certififcate)
        {
            if (Environment.GetEnvironmentVariable("GALLIUM_CERTIFICATE_PASSWORD") is string password)
            {
                opt.UseHttps(certififcate, password);
            }
            else
            {
                opt.UseHttps(certififcate);
            }
        }
        else
        {
            opt.UseHttps();
        }
    });
});

builder.Services.AddAuthentication(defaultScheme: "Basic").AddBasic();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
