using Sidekick.Common.Exceptions;

namespace Sidekick.Web;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> logger;
    private readonly RequestDelegate requestDelegate;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        RequestDelegate requestDelegate)
    {
        this.logger = logger;
        this.requestDelegate = requestDelegate;
    }

    /// <summary>
    ///     Method that is invoked on the middleware.
    /// </summary>
    /// <param name="httpContext">The http context of the current request.</param>
    /// <returns>A task.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await requestDelegate(httpContext);
        }
        catch (SidekickException ex)
        {
            logger.LogCritical(ex, "Unhandled exception.");
            throw;
        }
    }
}
