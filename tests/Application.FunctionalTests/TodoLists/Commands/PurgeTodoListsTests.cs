#if (UseAuthentication)
using CleanArchitecture.Application.Common.Exceptions;
#endif
using CleanArchitecture.Application.Common.Security;
#if (UseAuthentication)
using CleanArchitecture.Application.Data.TodoLists.Commands.CreateTodoList;
#endif
using CleanArchitecture.Application.Data.TodoLists.Commands.PurgeTodoLists;
#if (UseAuthentication)
using CleanArchitecture.Domain.Entities;
#endif

namespace CleanArchitecture.Application.FunctionalTests.TodoLists.Commands;

using static CleanArchitecture.Application.FunctionalTests.Testing;

public class PurgeTodoListsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new PurgeTodoListsCommand();

        command.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(command);

        #if (UseAuthentication)
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
        #else
        await action.Should().NotThrowAsync();
        #endif
    }

    #if (UseAuthentication)
    [Test]
    public async Task ShouldDenyNonAdministrator()
    {
        await RunAsDefaultUserAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        await action.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task ShouldAllowAdministrator()
    {
        await RunAsAdministratorAsync();

        var command = new PurgeTodoListsCommand();

        var action = () => SendAsync(command);

        await action.Should().NotThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task ShouldDeleteAllLists()
    {
        await RunAsAdministratorAsync();

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #1"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #2"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "New List #3"
        });

        await SendAsync(new PurgeTodoListsCommand());

        var count = await CountAsync<TodoList>();

        count.Should().Be(0);
    }
    #endif
}
