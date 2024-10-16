using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace GalliumPlus.WebService.Middleware.Authentication
{
    public static class ApiKey
    {
        public static bool Find([NotNullWhen(true)] out string? apiKey, IHeaderDictionary headers)
        {
            apiKey = null;

            if (headers.TryGetValue("X-API-Key", out StringValues headerValues))
            {
                apiKey = headerValues.ToString();
                return true;
            }

            return false;
        }
    }
}
