using LessonPlanningSystem.DatabaseLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder().Build();

using var scope = host.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<TimetableV4Context>();
foreach (var x in db.Departments.Include(x => x.Faculty)) {
    Console.WriteLine($"{x.Name} - {x.Faculty.Name}");
}


IHostBuilder CreateHostBuilder()
{
    var hostBuilder = new HostBuilder();
    hostBuilder.ConfigureAppConfiguration((_, builder) => {
        builder.AddUserSecrets<CurrentAssembly>();
    });
    hostBuilder.ConfigureServices((context, services) => {
        services.AddTimetableDb(context.Configuration.GetConnectionString("TimetableV4"), "8.0.28");
    });
    return hostBuilder;
}

internal class CurrentAssembly { }
