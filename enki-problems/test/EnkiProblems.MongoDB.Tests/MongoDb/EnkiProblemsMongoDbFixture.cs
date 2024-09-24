using System.Threading.Tasks;
using Testcontainers.MongoDb;
using Xunit;

namespace EnkiProblems.MongoDB;

public class EnkiProblemsMongoDbFixture : IAsyncLifetime
{
    private static readonly MongoDbContainer MongoDbContainer = new MongoDbBuilder().Build();

    public static string GetConnectionString() => MongoDbContainer.GetConnectionString();

    public Task InitializeAsync() => MongoDbContainer.StartAsync();

    public Task DisposeAsync() => MongoDbContainer.StopAsync();
}
