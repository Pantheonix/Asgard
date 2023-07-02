using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemsManagerTests : EnkiProblemsDomainTestBase
{
    private readonly EnkiProblemsTestData _testData;
    private readonly ProblemsManager _problemsManager;

    public ProblemsManagerTests()
    {
        _testData = GetRequiredService<EnkiProblemsTestData>();
        _problemsManager = GetRequiredService<ProblemsManager>();
    }

    [Fact]
    public async Task Should_Create_A_Valid_Problem()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemsManager.CreateAsync(
                _testData.ProblemName2,
                "Problem Brief",
                "Problem Description",
                "Problem Source Name",
                "Problem Author Name",
                1,
                1,
                1,
                IoTypeEnum.Standard,
                DifficultyEnum.Easy,
                3,
                new List<ProgrammingLanguageEnum> {ProgrammingLanguageEnum.CSharp}
            );

            problem.ShouldNotBeNull();
            problem.Name.ShouldBe(_testData.ProblemName2);
        });
    }

    [Fact]
    public async Task Should_Not_Create_A_Problem_With_Already_Existing_Name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _problemsManager.CreateAsync(
                    _testData.ProblemName1,
                    "Problem Brief",
                    "Problem Description",
                    "Problem Source Name",
                    "Problem Author Name",
                    1,
                    1,
                    1,
                    IoTypeEnum.Standard,
                    DifficultyEnum.Easy,
                    3,
                    new List<ProgrammingLanguageEnum> { ProgrammingLanguageEnum.CSharp }
                );
            });
        });
    }
}
