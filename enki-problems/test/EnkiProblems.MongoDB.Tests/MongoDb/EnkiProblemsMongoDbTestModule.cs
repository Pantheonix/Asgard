using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace EnkiProblems.MongoDB;

[DependsOn(typeof(EnkiProblemsTestBaseModule), typeof(EnkiProblemsMongoDbModule))]
public class EnkiProblemsMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var connectionString =
            $"{EnkiProblemsMongoDbFixture.GetConnectionString().EnsureEndsWith('/')}Db_{Guid.NewGuid():N}?authSource=admin";

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = connectionString;
        });
    }
}
