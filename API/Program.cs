using GalliumPlusAPI.Database;
using GalliumPlusAPI.Database.Implementations.FakeDatabase;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IMasterDao, FakeDao>();

// Configuration du sérialiseur JSON
builder.Services.Configure<JsonOptions>(jsonOptions => {
    JsonSerializerOptions serializerOptions = jsonOptions.JsonSerializerOptions;

    // accepte uniquement le format nombre JSON pour les entier et les floats
    serializerOptions.NumberHandling = JsonNumberHandling.Strict;

    // accepte les virgules en fin de liste / d'objet
    serializerOptions.AllowTrailingCommas = true;

    // garde les noms de propriétés tels quels
    serializerOptions.PropertyNamingPolicy = null;

    // sérialise les énumérations sous forme de texte
    serializerOptions.Converters.Add(new JsonStringEnumConverter());
});

#if DEBUG
#else
// configuration HTTP/HTTPS
builder.WebHost.ConfigureKestrel(opt => {
    int httpPort;
    if (!Int32.TryParse(Environment.GetEnvironmentVariable("HTTP_PORT"), out httpPort))
    {
        httpPort = 5080;
    }

    int httpsPort;
    if (!Int32.TryParse(Environment.GetEnvironmentVariable("HTTPS_PORT"), out httpsPort))
    {
        httpsPort = 5443;
    }

    opt.ListenAnyIP(httpPort);
    opt.ListenAnyIP(httpsPort, opt =>
    {
        opt.UseHttps(
            Environment.GetEnvironmentVariable("CERTIFICATE_FILE") ?? "",
            Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD" ?? "")
        );
    });
});
#endif

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
