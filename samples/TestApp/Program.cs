using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using LessonPlanningSystem.DatabaseLayer;
using LessonPlanningSystem.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// var host = CreateHostBuilder().Build();
//
// using var scope = host.Services.CreateScope();
//
// var db = scope.ServiceProvider.GetRequiredService<TimetableV4Context>();
// foreach (var x in db.Departments.Include(x => x.Faculty)) {
//     Console.WriteLine($"{x.Name} - {x.Faculty.Name}");
// }
// var conDict = new ConcurrentDictionary<int, int>();

var arr = new[] { 1, 2, 3, 4, 5 };
var encoded  = Encode(string.Join(',', arr));
Console.WriteLine(encoded);
Console.WriteLine(Decode(encoded));

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

static string Decode(string base64EncodedData)
{
    var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
    return Encoding.UTF8.GetString(base64EncodedBytes);
}
static string Encode(string plainText)
{
    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
    return Convert.ToBase64String(plainTextBytes);
}

internal class CurrentAssembly { }
