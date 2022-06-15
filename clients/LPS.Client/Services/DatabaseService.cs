using System;
using System.Threading.Tasks;
using LPS.Client.Models;
using LPS.DatabaseLayer;
using Microsoft.Extensions.DependencyInjection;

namespace LPS.Client.Services;

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
}
