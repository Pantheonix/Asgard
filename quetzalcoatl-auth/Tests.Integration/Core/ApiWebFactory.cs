namespace Tests.Integration.Core;

public class ApiWebFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(AppDbContext));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_database.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync() => await _database.StartAsync();

    public new async Task DisposeAsync() => await _database.StopAsync();
}
