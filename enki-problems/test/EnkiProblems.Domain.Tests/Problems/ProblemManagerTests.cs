using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemManagerTests : EnkiProblemsDomainTestBase
{
    private readonly EnkiProblemsTestData _testData;
    private readonly ProblemManager _problemManager;
    private readonly IRepository<Problem, Guid> _problemRepository;

    public ProblemManagerTests()
    {
        _testData = GetRequiredService<EnkiProblemsTestData>();
        _problemManager = GetRequiredService<ProblemManager>();
        _problemRepository = GetRequiredService<IRepository<Problem, Guid>>();
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
            problem.Brief.ShouldBe(_testData.ProblemBrief2);
            problem.Description.ShouldBe(_testData.ProblemDescription2);
            problem.IsPublished.ShouldBeFalse();
            problem.Origin.SourceName.ShouldBe(_testData.ProblemSourceName2);
            problem.Origin.AuthorName.ShouldBe(_testData.ProblemAuthorName2);
            problem.Limit.Time.ShouldBe(_testData.ProblemTimeLimit2);
            problem.Limit.StackMemory.ShouldBe(_testData.ProblemStackMemoryLimit2);
            problem.Limit.TotalMemory.ShouldBe(_testData.ProblemTotalMemoryLimit2);
            problem.IoType.ShouldBe(_testData.ProblemIoType2);
            problem.Difficulty.ShouldBe(_testData.ProblemDifficulty2);
            problem.NumberOfTests.ShouldBe(_testData.ProblemNumberOfTests2);
            problem.ProgrammingLanguages.ShouldBe(_testData.ProblemProgrammingLanguages2);
            problem.ProposerId.ShouldBe(_testData.ProblemProposerId2);
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

    [Fact]
    public async Task Should_Update_A_Valid_Problem()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemRepository.GetAsync(
                _testData.ProblemId1
            );
            await _problemManager.UpdateAsync(
                problem,
                _testData.ProblemName2,
                _testData.ProblemBrief2,
                _testData.ProblemDescription2,
                _testData.ProblemSourceName2,
                _testData.ProblemAuthorName2,
                _testData.ProblemTimeLimit2,
                _testData.ProblemTotalMemoryLimit2,
                _testData.ProblemStackMemoryLimit2,
                _testData.ProblemIoType2,
                _testData.ProblemDifficulty2,
                _testData.ProblemNumberOfTests2,
                _testData.ProblemProgrammingLanguages2
            );
            await _problemRepository.UpdateAsync(problem);
        });

        var problem = await _problemRepository.GetAsync(
            _testData.ProblemId1
        );
        problem.ShouldNotBeNull();
        problem.Name.ShouldBe(_testData.ProblemName2);
        problem.Brief.ShouldBe(_testData.ProblemBrief2);
        problem.Description.ShouldBe(_testData.ProblemDescription2);
        problem.IsPublished.ShouldBeFalse();
        problem.Origin.SourceName.ShouldBe(_testData.ProblemSourceName2);
        problem.Origin.AuthorName.ShouldBe(_testData.ProblemAuthorName2);
        problem.Limit.Time.ShouldBe(_testData.ProblemTimeLimit2);
        problem.Limit.StackMemory.ShouldBe(_testData.ProblemStackMemoryLimit2);
        problem.Limit.TotalMemory.ShouldBe(_testData.ProblemTotalMemoryLimit2);
        problem.IoType.ShouldBe(_testData.ProblemIoType2);
        problem.Difficulty.ShouldBe(_testData.ProblemDifficulty2);
        problem.NumberOfTests.ShouldBe(_testData.ProblemNumberOfTests2);
        problem.ProgrammingLanguages.ShouldBe(_testData.ProblemProgrammingLanguages2);
        problem.ProposerId.ShouldBe(_testData.ProblemProposerId2);
    }

    [Fact]
    public async Task Should_Not_Update_A_Problem_With_Already_Existing_Name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemRepository.GetAsync(
                _testData.ProblemId1
            );
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _problemManager.UpdateAsync(
                    problem,
                    _testData.ProblemName1,
                    _testData.ProblemBrief2,
                    _testData.ProblemDescription2,
                    _testData.ProblemSourceName2,
                    _testData.ProblemAuthorName2,
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

    // TODO: Run this test after implementing the Publish method in the Problem entity
    // [Fact]
    // public async Task Should_Not_Update_A_Published_Problem()
    // {
    //     await WithUnitOfWorkAsync(async () =>
    //     {
    //         var problem = await _problemRepository.GetAsync(
    //             _testData.ProblemId1
    //         );
    //         problem.Publish();
    //         await Assert.ThrowsAsync<BusinessException>(async () =>
    //         {
    //             await _problemManager.UpdateAsync(
    //                 problem,
    //                 _testData.ProblemName2,
    //                 _testData.ProblemBrief2,
    //                 _testData.ProblemDescription2,
    //                 _testData.ProblemSourceName2,
    //                 _testData.ProblemAuthorName2,
    //                 _testData.ProblemTimeLimit2,
    //                 _testData.ProblemTotalMemoryLimit2,
    //                 _testData.ProblemStackMemoryLimit2,
    //                 _testData.ProblemIoType2,
    //                 _testData.ProblemDifficulty2,
    //                 _testData.ProblemNumberOfTests2,
    //                 _testData.ProblemProgrammingLanguages2
    //             );
    //         });
    //     });
    // }
}
