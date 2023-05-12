using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;

namespace MusicPlayer;

public static class IocConfig
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }
}