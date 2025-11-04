using Serilog.Context;

namespace UsersService.Middlewares;

public class LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LogMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;
        
        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var XCorrelationId))
        {
            correlationId = XCorrelationId.ToString();
            _logger.LogInformation("Incoming correlation id: {XCorrelationId}", XCorrelationId.ToString());
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
            _logger.LogInformation("Incoming CorrelationId not found. Setting CorrleationId to: {XCorrelationId}", correlationId);

        }

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {

            await _next(context);
        }


    }
}
