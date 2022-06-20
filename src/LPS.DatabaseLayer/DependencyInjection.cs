using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LPS.DatabaseLayer;

public static class DependencyInjection
{
    public static void AddTimetableDb(this IServiceCollection services, string connectionString, Version mysqlVersion)
    {
        services.AddDbContext<TimetableV4Context>(options => options.UseMySql(connectionString,
            new MySqlServerVersion(mysqlVersion)));
    }
}
