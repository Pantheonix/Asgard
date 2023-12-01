namespace Bootstrapper.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
        where T : DbContext
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<T>)
        );
        if (descriptor != null)
            services.Remove(descriptor);
    }

    public static void ApplyMigrations<T>(this IServiceCollection services)
        where T : DbContext
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<T>();

        context.Database.EnsureCreated();
    }
}
