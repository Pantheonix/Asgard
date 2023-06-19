using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace EnkiProblems.MongoDB;

[DependsOn(
    typeof(EnkiProblemsTestBaseModule),
    typeof(EnkiProblemsMongoDbModule)
    )]
public class EnkiProblemsMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var stringArray = EnkiProblemsMongoDbFixture.ConnectionString.Split('?');
        var connectionString = stringArray[0].EnsureEndsWith('/') +
                                   "Db_" +
                               Guid.NewGuid().ToString("N") + "/?" + stringArray[1];

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = connectionString;
        });
    }
}
