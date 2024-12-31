using System.Data.Common;
#if (UseAuthentication)
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
#endif
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
#if (UseAuthentication)
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace CleanArchitecture.Application.FunctionalTests;

using static Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;
    private readonly string _connectionString;

    public CustomWebApplicationFactory(DbConnection connection, string connectionString)
    {
        _connection = connection;
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
#if (UseAspire)
        builder.UseSetting("ConnectionStrings:CleanArchitectureDb", _connectionString);
#endif
        builder.ConfigureTestServices(services =>
        {
#if (UseAuthentication)
            services
                .RemoveAll<IUser>()
                .AddTransient(provider => Mock.Of<IUser>(s => s.Id == GetUserId()));
#endif
#if (!UseAspire || UseSqlite)
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
    #if (UsePostgreSQL)
                    options.UseNpgsql(_connection);
    #elif (UseSqlite)
                    options.UseSqlite(_connection);
    #else
                    options.UseSqlServer(_connection);
    #endif
                });
#endif
        });
    }
}
