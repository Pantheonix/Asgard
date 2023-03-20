
namespace Tests.Integration.Core;

public class ApiWebFactory : WebApplicationFactory<IProgramMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<AppDbContext>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_database.GetConnectionString());
            });
            services.EnsureDbCreated<AppDbContext>();
        });
    }

    public async Task InitializeAsync() => await _database.StartAsync();

    public new async Task DisposeAsync() => await _database.StopAsync();
}
