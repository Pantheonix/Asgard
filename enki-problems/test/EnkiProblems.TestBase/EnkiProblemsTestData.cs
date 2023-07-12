using System;
using EnkiProblems.Problems;
using Volo.Abp.DependencyInjection;

namespace EnkiProblems;

public class EnkiProblemsTestData : ISingletonDependency
{
    /* PROBLEMS */
    public Guid ProblemId1 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public string ProblemName1 { get; } = "Problem Name 1";

    public string ProblemBrief1 { get; } = "Problem Brief 1";

    public string ProblemDescription1 { get; } = "Problem Description 1";

    public string ProblemSourceName1 { get; } = "Problem Source Name 1";

    public string ProblemAuthorName1 { get; } = "Problem Author Name 1";

    public Guid ProblemProposerId1 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public int ProblemTimeLimit1 { get; } = 1;

    public int ProblemStackMemoryLimit1 { get; } = 1;

    public int ProblemTotalMemoryLimit1 { get; } = 1;

    public int ProblemNumberOfTests1 { get; } = 2;

    public IoTypeEnum ProblemIoType1 { get; } = IoTypeEnum.Standard;

    public DifficultyEnum ProblemDifficulty1 { get; } = DifficultyEnum.Easy;

    public ProgrammingLanguageEnum[] ProblemProgrammingLanguages1 { get; } =
        { ProgrammingLanguageEnum.CSharp };

    public Guid ProblemId2 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");

    public string ProblemName2 { get; } = "Problem Name 2";

    public string ProblemBrief2 { get; } = "Problem Brief 2";

    public string ProblemDescription2 { get; } = "Problem Description 2";

    public string ProblemSourceName2 { get; } = "Problem Source Name 2";

    public string ProblemAuthorName2 { get; } = "Problem Author Name 2";

    public Guid ProblemProposerId2 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");

    public int ProblemTimeLimit2 { get; } = 2;

    public int ProblemStackMemoryLimit2 { get; } = 2;

    public int ProblemTotalMemoryLimit2 { get; } = 2;

    public int ProblemNumberOfTests2 { get; } = 2;

    public IoTypeEnum ProblemIoType2 { get; } = IoTypeEnum.Standard;

    public DifficultyEnum ProblemDifficulty2 { get; } = DifficultyEnum.Medium;

    public ProgrammingLanguageEnum[] ProblemProgrammingLanguages2 { get; } =
        { ProgrammingLanguageEnum.Rust };

    /* USERS */
    public Guid NormalUserId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public string[] NormalUserRoles { get; } = { "User" };

    public Guid ProposerUserId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");

    public string[] ProposerUserRoles { get; } = { "User", "Proposer" };
}
