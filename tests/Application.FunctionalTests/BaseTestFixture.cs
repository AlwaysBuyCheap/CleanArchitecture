namespace CleanArchitecture.Application.FunctionalTests;

using static CleanArchitecture.Application.FunctionalTests.Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
