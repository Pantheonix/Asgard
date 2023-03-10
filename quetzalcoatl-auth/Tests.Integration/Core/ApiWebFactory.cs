namespace Tests.Integration.Core;

public class ApiWebFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _database =
        new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(
                new MsSqlTestcontainerConfiguration
                {
                    Database = "TestDb",
                    Username = "TestUser",
                    Password = "TestPassword"
                }
            )
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(AppDbContext));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_database.ConnectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.StopAsync();
    }
}
