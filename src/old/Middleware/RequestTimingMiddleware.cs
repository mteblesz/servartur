using System.Diagnostics;

namespace servartur.Middleware;

internal class RequestTimingMiddleware : IMiddleware
{
    private readonly ILogger<RequestTimingMiddleware> _logger;
    private readonly Stopwatch _stopwatch;

    public RequestTimingMiddleware(ILogger<RequestTimingMiddleware> logger)
    {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _stopwatch.Start();
        try
        {
            await next.Invoke(context);
        }
        finally
        {
            _stopwatch.Stop();
            var elapsed_seconds = _stopwatch.Elapsed.TotalSeconds;
            if (elapsed_seconds > 4)
            {
                _logger.LogInformation($"Request took {elapsed_seconds:F2} seconds: " +
                    $"{context.Request.Method} {context.Request.Path}");
            }
        }
    }
}
