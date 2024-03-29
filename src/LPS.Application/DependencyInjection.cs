﻿using Microsoft.Extensions.DependencyInjection;

namespace LPS.Application;

public static class DependencyInjection
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<TimetableService>();
        services.AddAutoMapper(typeof(DependencyInjection));
    }
}
