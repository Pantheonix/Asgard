using System;
using Volo.Abp.DependencyInjection;

namespace EnkiProblems;

public class EnkiProblemsTestData : ISingletonDependency
{
    /* PROBLEMS */
    public Guid ProblemId1 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public string ProblemName1 { get; } = "Problem Name 1";
    
    public Guid ProblemId2 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public string ProblemName2 { get; } = "Problem Name 2";
}
