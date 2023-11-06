using System.Security.Claims;
using Applander.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Applendar.API.Features;

public class LogUserLastActivityMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    private readonly ApplendarDbContext _dbContext;

    public LogUserLastActivityMiddleware(
        ApplendarDbContext dbContext,
        ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
            {
                string? externalUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.Debug($"User {externalUserId} performed an action at {DateTime.UtcNow}.");

                var user = await _dbContext.ApplendarUsers.SingleOrDefaultAsync(x => x.ExternalId == externalUserId);

                if (user is null)
                {
                    throw new KeyNotFoundException($"User {externalUserId} not registered");
                }

                user.LogActivity();

                await _dbContext.SaveChangesAsync();
            }
        }
        catch (KeyNotFoundException exception)
        {
            _logger.Debug(exception.Message);
        }
        catch (Exception exception) when (exception is InvalidOperationException
                                                       or DbUpdateException
                                                       or DbUpdateConcurrencyException)
        {
            _logger.Error(exception.Message);
        }
        finally
        {
            await next(context);
        }
    }
}