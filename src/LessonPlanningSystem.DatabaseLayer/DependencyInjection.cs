﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LessonPlanningSystem.DatabaseLayer;

public static class DependencyInjection
{
    public static void AddTimetableDb(this IServiceCollection services, string connectionString, string mysqlVersion)
    {
        services.AddDbContext<TimetableV4Context>(options => options.UseMySql(connectionString,
            new MySqlServerVersion(mysqlVersion)));
    }
}