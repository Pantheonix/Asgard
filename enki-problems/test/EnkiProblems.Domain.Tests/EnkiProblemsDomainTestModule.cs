using EnkiProblems.MongoDB;
using Volo.Abp.Modularity;

namespace EnkiProblems;

[DependsOn(typeof(EnkiProblemsMongoDbTestModule))]
public class EnkiProblemsDomainTestModule : AbpModule { }
