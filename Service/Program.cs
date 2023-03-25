using GalliumPlus.WebApi.Data;
using GalliumPlus.WebApi.Data.Implementations.FakeDatabase;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IMasterDao, FakeDaoFacade>();

// accepte uniquement le format nombre JSON pour les entier et les floats
Controller.JsonOptions.NumberHandling = JsonNumberHandling.Strict;
// accepte les virgules en fin de liste / d'objet
Controller.JsonOptions.AllowTrailingCommas = true;
// garde les noms de propriétés tels quels
Controller.JsonOptions.PropertyNamingPolicy = null;
// sérialise les énumérations sous forme de texte
Controller.JsonOptions.Converters.Add(new JsonStringEnumConverter());

// configuration HTTP/HTTPS
builder.WebHost.ConfigureKestrel(opt => {
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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
