using CleanArchitecture.Application.Common.Behaviours;
#if (UseAuthentication)
using CleanArchitecture.Application.Common.Interfaces;
#endif
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateTodoItemCommand>> _logger = null!;
#if (UseAuthentication)
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;
#endif

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateTodoItemCommand>>();
#if (UseAuthentication)
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
#endif
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
#if (UseAuthentication)
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _user.Object, _identityService.Object);
#else
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object);
#endif

        await requestLogger.Process(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new CancellationToken());

#if (UseAuthentication)
        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
#endif
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
#if (UseAuthentication)
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _user.Object, _identityService.Object);
#else
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object);
#endif

        await requestLogger.Process(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new CancellationToken());

#if (UseAuthentication)
        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
#endif
    }
}
