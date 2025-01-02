#if (UseAuthentication)
using CleanArchitecture.Application.Common.Interfaces;
#endif
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    #if (UseAuthentication)
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    #endif

    #if (UseAuthentication)
    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }
    #else
    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }
    #endif

    #if (UseAuthentication)
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
    #else
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("CleanArchitectureNoAuth Request: {Name} {@Request}", requestName, request);

        return Task.CompletedTask;
    }
    #endif
}
