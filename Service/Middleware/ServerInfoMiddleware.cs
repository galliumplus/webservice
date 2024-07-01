namespace GalliumPlus.WebApi.Middleware
{
    public class ServerInfoMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.Headers.Add("X-Gallium-Version", ServerInfo.Current.Version);
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

        private string version = $"unknown (unknown/{configuration})";

#if TEST
        private const string configuration = "test";
#elif DEBUG
        private const string configuration = "debug";
#else
        private const string configuration = "release";
#endif

        public string Version => this.version;


        private ServerInfo() { }

        public void SetVersion(int major, int minor, int patch, string stage = "unknown")
        {
            string build = Builtins.CompileDateTime.ToString("yyMMddHHmm");
            this.version = $"{major}.{minor}.{patch}.{build} ({stage}/{configuration})";
        }

        public override string ToString()
        {
            return "   ___        _ _ _                   _    \n"
                 + "  / __|  __ _| | (_)_   _ _ __ ___  _| |_  \n"
                 + " | |  _ / _` | | | | | | | '_ ` _ \\'_   _|\n"
                 + " | |_| | (_| | | | | |_| | | | | | | |_|   \n"
                 + "  \\____|\\__,_|_|_|_|\\__,_|_| |_| |_|    \n"
                 + $"\n Gallium+ Web API Server v{this.Version}\n";
        }
    }
}

public static partial class Builtins
{
    // astuce pour récupérer l'heure du build
    private static readonly long CompileTime = 0;
    public static DateTime CompileDateTime => new DateTime(CompileTime, DateTimeKind.Utc);
}

