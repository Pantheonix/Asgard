using EnkiProblems.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace EnkiProblems.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(EnkiProblemsMongoDbModule),
    typeof(EnkiProblemsApplicationContractsModule)
)]
public class EnkiProblemsDbMigratorModule : AbpModule { }
