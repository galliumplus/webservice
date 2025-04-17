namespace GalliumPlus.WebService.Middleware;

public class ServerInfoMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Append("X-Gallium-Version", ServerInfo.Current.PrettyVersion);
        await next.Invoke(context);
    }
}

public static class ServerInfoMiddlewareExtensions
{
    public static IServiceCollection AddServerInfo(this IServiceCollection services) =>
        services.AddSingleton<ServerInfoMiddleware>();

    public static IApplicationBuilder UseServerInfo(this IApplicationBuilder app) =>
        app.UseMiddleware<ServerInfoMiddleware>();
}

public class ServerInfo
{
    private static ServerInfo? current = null;

    public static ServerInfo Current
    {
        get
        {
            if (current == null)
            {
                current = new ServerInfo();
            }

            return current;
        }
    }

    private string prettyVersion = $"unknown (unknown/{CONFIGURATION})";
    private string compactVersion = "unknown";

#if TEST
        private const string CONFIGURATION = "test";
#elif DEBUG
    private const string CONFIGURATION = "debug";
#else
        private const string CONFIGURATION = "release";
#endif

    public string PrettyVersion => this.prettyVersion;
        
    public string CompactVersion => this.compactVersion;

    private ServerInfo() { }

    public void SetVersion(int major, int minor, int patch, string stage = "unknown")
    {
        string build = Builtins.CompileDateTime.ToString("yyMMddHHmm");
        this.prettyVersion = $"{major}.{minor}.{patch}.{build} ({stage}/{CONFIGURATION})";
        this.compactVersion = $"{major}.{minor}.{patch}-{CONFIGURATION[0]}";
    }

    public override string ToString()
    {
        return "   ___        _ _ _                   _    \n"
               + "  / __|  __ _| | (_)_   _ _ __ ___  _| |_  \n"
               + " | |  _ / _` | | | | | | | '_ ` _ \\'_   _|\n"
               + " | |_| | (_| | | | | |_| | | | | | | |_|   \n"
               + "  \\____|\\__,_|_|_|_|\\__,_|_| |_| |_|    \n"
               + $"\n Gallium+ Web API Server v{this.PrettyVersion}\n";
    }
}

#pragma warning disable CA1050

public static partial class Builtins
{
    // astuce pour récupérer l'heure du build
    private static readonly long CompileTime;
    public static DateTime CompileDateTime => new(CompileTime, DateTimeKind.Utc);
}

#pragma warning restore CA1050