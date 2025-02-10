using MRA.DB;

namespace MineralResourceAccounting.Configuration;

public static class ConfigureService
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddTransient<MineralRepository>()
            .AddOpenApi()
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "MineralResourceAccounting", Version = "v1" });
            });
        return services;
    }
}