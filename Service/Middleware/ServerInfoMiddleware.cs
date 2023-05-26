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

        private string version = "N/A";

        public string Version { get => this.version; set => this.version = value; }

        private ServerInfo() { }

        public override string ToString()
        {
            return $"Gallium+ Web API Server v{this.version}";
        }
    }
}
