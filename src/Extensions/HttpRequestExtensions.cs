namespace Linn.Common.Service.Extensions
{
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtensions
    {
        public static string GetContentType(this HttpRequest req) 
            => (!req.Headers.ContainsKey("Accept") 
                    ? req.ContentType 
                      ?? string.Empty : req.Headers["Accept"].FirstOrDefault<string>()) ?? string.Empty;
    }
}
