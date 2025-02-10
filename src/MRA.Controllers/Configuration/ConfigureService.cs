using MRA.DB;
using MySqlConnector;

namespace MineralResourceAccounting.Configuration;

public static class ConfigureService
{
    public static IServiceCollection AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services
            .AddTransient<MineralRepository>()
            .AddOpenApi()
            .AddMySqlDataSource(builder.Configuration.GetConnectionString("AppConnection")!)
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "MineralResourceAccounting", Version = "v1" });
            });
        return services;
    }
}