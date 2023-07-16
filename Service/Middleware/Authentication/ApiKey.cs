using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace GalliumPlus.WebApi.Middleware.Authentication
{
    public static class ApiKey
    {
        public static bool Find([NotNullWhen(true)] out string? apiKey, IHeaderDictionary headers)
        {
            apiKey = null;

            if (headers.TryGetValue("X-API-Key", out StringValues headerValues))
            {
                apiKey = headerValues;
                return true;
            }

            return false;
        }
    }
}
