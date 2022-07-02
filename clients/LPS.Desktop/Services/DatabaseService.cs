using System;
using System.Threading.Tasks;
using LPS.Application;
using LPS.DatabaseLayer;
using LPS.Desktop.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LPS.Desktop.Services;

public class DatabaseService
{
    public static async Task UsingContextAsync(ConnectionDetails details, Func<TimetableV4Context, Task> action)
    {
        var services = new ServiceCollection();
        services.AddTimetableDb(details.GetConnectionString(), details.GetMysqlVersion());
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetService<TimetableV4Context>()!;
        await action.Invoke(context);
    }
    
    public static async Task UsingTimetableServiceAsync(ConfigurationDetails details, Func<TimetableService, Task> action)
    {
        var services = new ServiceCollection();
        var connection = details.ConnectionDetails;
        services.AddTimetableDb(connection.GetConnectionString(), connection.GetMysqlVersion());
        services.AddApplicationDependencies();
        services.AddSingleton(details.PlanConfiguration);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var service = scope.ServiceProvider.GetService<TimetableService>()!;
        await action.Invoke(service);
    }
}
