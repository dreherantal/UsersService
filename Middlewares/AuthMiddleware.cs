using Microsoft.Extensions.Options;
using UsersService.Options;

namespace UsersService.Middlewares;

public class AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger, IOptions<APIOptions> APIOptions)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<AuthMiddleware> _logger = logger;
    private readonly string apiKey = APIOptions.Value.UsersServiceAPIKey;

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

        if (context.Request.Headers.TryGetValue("X-UserID", out var XUserID))
        {
            if (int.TryParse(XUserID.ToString(), out int UserId) && (UserId > 0))
            {
                context.Items["UserId"] = UserId;
                _logger.LogInformation("Parsed X-UserID from request header: {UserID}", UserId);
            }
            else
            {
                _logger.LogInformation("Could not parse UserID from X-UserID value. X-UserID value: {UserID}", XUserID.ToString());
            }
        }
        else
        {
            _logger.LogInformation("Could not find X-UserID in request header or it's value is empty.");
        }



        if (context.Request.Headers.TryGetValue("X-API-Key", out var XAPIKey))
        {
            string requestAPIKey = XAPIKey.ToString();

            if (requestAPIKey.Equals(apiKey))
            {
                _logger.LogInformation("X-API-Key verified");

                await _next(context);
            }
            else
            {
                _logger.LogWarning("Could not verify X-API-Key: {XAPIKey}", requestAPIKey);

                await UnauthorizedResponseAsync(context);
            }

        }
        else
        {
            _logger.LogWarning("Could not find X-API-Key in request header.");

            await UnauthorizedResponseAsync(context);
        }

    }
    private static async Task UnauthorizedResponseAsync(HttpContext context)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Unauthorized");
    }
}
