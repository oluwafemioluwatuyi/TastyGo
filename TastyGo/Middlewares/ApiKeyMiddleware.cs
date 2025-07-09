using Microsoft.AspNetCore.Http;
namespace TastyGo.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration["ApiKey"] ?? throw new ArgumentNullException("ApiKey is missing from configuration.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            // Skip validation if endpoint has [AllowAnonymousApiKey] attribute
            if (endpoint?.Metadata?.GetMetadata<AllowAnonymousApiKeyAttribute>() != null)
            {
                await _next(context);
                return;
            }

            var providedKey = context.Request.Headers["X-API-Key"].FirstOrDefault();

            if (providedKey != _apiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid API Key");
                return;
            }

            await _next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousApiKeyAttribute : Attribute
    {
    }
}
