using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LPS.DatabaseLayer;

public static class DependencyInjection
{
    public static void AddTimetableDb(this IServiceCollection services, string connectionString, Version mysqlVersion)
    {
        services.AddDbContext<TimetableContext>(options => options.UseMySql(connectionString,
            new MySqlServerVersion(mysqlVersion)));
    }

    public static void AddTimetableDb(this IServiceCollection services, string connectionString, string mysqlVersion)
    {
        services.AddDbContext<TimetableContext>(options => options.UseMySql(connectionString,
            new MySqlServerVersion(mysqlVersion)));
    }
}
