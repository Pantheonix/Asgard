using Volo.Abp.Modularity;

namespace EnkiProblems;

[DependsOn(
    typeof(EnkiProblemsApplicationModule),
    typeof(EnkiProblemsDomainTestModule)
    )]
public class EnkiProblemsApplicationTestModule : AbpModule
{

}
