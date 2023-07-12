using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemManagerTests : EnkiProblemsDomainTestBase
{
    private readonly EnkiProblemsTestData _testData;
    private readonly ProblemManager _problemManager;

    public ProblemManagerTests()
    {
        _testData = GetRequiredService<EnkiProblemsTestData>();
        _problemManager = GetRequiredService<ProblemManager>();
    }

    [Fact]
    public async Task Should_Create_A_Valid_Problem()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemManager.CreateAsync(
                _testData.ProblemName2,
                _testData.ProblemBrief2,
                _testData.ProblemDescription2,
                _testData.ProblemSourceName2,
                _testData.ProblemAuthorName2,
                _testData.ProblemProposerId2,
                _testData.ProblemTimeLimit2,
                _testData.ProblemTotalMemoryLimit2,
                _testData.ProblemStackMemoryLimit2,
                _testData.ProblemIoType2,
                _testData.ProblemDifficulty2,
                _testData.ProblemNumberOfTests2,
                _testData.ProblemProgrammingLanguages2
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
                await _problemManager.CreateAsync(
                    _testData.ProblemName1,
                    _testData.ProblemBrief2,
                    _testData.ProblemDescription2,
                    _testData.ProblemSourceName2,
                    _testData.ProblemAuthorName2,
                    _testData.ProblemProposerId2,
                    _testData.ProblemTimeLimit2,
                    _testData.ProblemTotalMemoryLimit2,
                    _testData.ProblemStackMemoryLimit2,
                    _testData.ProblemIoType2,
                    _testData.ProblemDifficulty2,
                    _testData.ProblemNumberOfTests2,
                    _testData.ProblemProgrammingLanguages2
                );
            });
        });
    }
}
