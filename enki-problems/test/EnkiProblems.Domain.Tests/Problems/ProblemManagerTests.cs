using System;
using System.Linq;
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
                _testData.ProblemDifficulty2
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
            problem.Tests.Count.ShouldBe(0);
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
                    _testData.ProblemDifficulty2
                );
            });
        });
    }

    [Fact]
    public async Task Should_Update_A_Valid_Problem()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
            var updatedProblem = await _problemManager.UpdateAsync(
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
                null
            );
            await _problemRepository.UpdateAsync(updatedProblem);
        });

        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
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
        problem.Tests.Count.ShouldBe(_testData.ProblemNumberOfTests1);
        problem.ProposerId.ShouldBe(_testData.ProblemProposerId2);
    }

    [Fact]
    public async Task Should_Not_Update_A_Problem_With_Already_Existing_Name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
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
                    null
                );
            });
        });
    }

    [Fact]
    public async Task Should_Not_Update_A_Published_Problem()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var problem = await _problemRepository.GetAsync(
                _testData.ProblemId1
            );
            
            problem.Publish();
            
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
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
                    null
                );
            });
        });
    }

    [Fact]
    public async Task Should_Add_A_New_Valid_Test()
    {
        // Arrange
        var problem = await _problemRepository.GetAsync(_testData.ProblemId3);

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedProblem = _problemManager.AddTest(
                problem,
                _testData.TestId1,
                _testData.TestScore1,
                _testData.TestInputLink1,
                _testData.TestOutputLink1
            );
            await _problemRepository.UpdateAsync(updatedProblem);
        });

        // Assert
        var problemWithTests = await _problemRepository.GetAsync(_testData.ProblemId3);
        problemWithTests.Tests.Count.ShouldBe(1);
        problemWithTests.Tests.First().Id.ShouldBe(_testData.TestId1);
        problemWithTests.Tests.First().Score.ShouldBe(_testData.TestScore1);
    }

    [Fact]
    public async Task Should_Not_Add_A_Test_With_Limit_Exceeding_Score()
    {
        // Arrange
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);

        // Act & Assert
        Assert.Throws<BusinessException>(() =>
        {
            _problemManager.AddTest(
                problem,
                _testData.TestId2,
                _testData.LimitExceedingTestScore,
                _testData.TestInputLink1,
                _testData.TestOutputLink1
            );
        });
    }

    [Fact]
    public async Task Should_Update_An_Existing_Test()
    {
        // Arrange
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
        var test = problem.Tests.First();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedProblem = _problemManager.UpdateTest(
                problem,
                test.Id,
                _testData.TestScore2,
                _testData.TestInputLink1,
                _testData.TestOutputLink1
            );
            await _problemRepository.UpdateAsync(updatedProblem);
        });

        // Assert
        var problemWithTests = await _problemRepository.GetAsync(_testData.ProblemId1);
        problemWithTests.Tests.Count.ShouldBe(1);
        problemWithTests.Tests.First().Id.ShouldBe(test.Id);
        problemWithTests.Tests.First().Score.ShouldBe(_testData.TestScore2);
    }

    [Fact]
    public async Task Should_Not_Update_Non_Existing_Test()
    {
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);

        Assert.Throws<BusinessException>(() =>
        {
            _problemManager.UpdateTest(
                problem,
                _testData.TestId2,
                _testData.TestScore2,
                _testData.TestInputLink1,
                _testData.TestOutputLink1
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Test_With_Limit_Exceeding_Score()
    {
        // Arrange
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
        var test = problem.Tests.First();

        // Act & Assert
        Assert.Throws<BusinessException>(() =>
        {
            _problemManager.UpdateTest(
                problem,
                test.Id,
                _testData.LimitExceedingTestScore,
                _testData.TestInputLink1,
                _testData.TestOutputLink1
            );
        });
    }

    [Fact]
    public async Task Should_Remove_An_Existing_Test()
    {
        // Arrange
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
        var test = problem.Tests.First();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedProblem = _problemManager.RemoveTest(problem, test.Id);
            await _problemRepository.UpdateAsync(updatedProblem);
        });

        // Assert
        var problemWithTests = await _problemRepository.GetAsync(_testData.ProblemId1);
        problemWithTests.Tests.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Not_Remove_Non_Existing_Test()
    {
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);

        Assert.Throws<BusinessException>(() =>
        {
            _problemManager.RemoveTest(problem, _testData.TestId2);
        });
    }

    [Fact]
    public async Task Should_Publish_Valid_Problem()
    {
        var problem = await _problemRepository.GetAsync(_testData.ProblemId1);
        
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedProblem = _problemManager.Publish(problem);
            await _problemRepository.UpdateAsync(updatedProblem);
        });
        
        var problemWithTests = await _problemRepository.GetAsync(_testData.ProblemId1);
        problemWithTests.IsPublished.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Not_Publish_Invalid_Problem()
    {
        var problem = await _problemRepository.GetAsync(_testData.ProblemId3);
        
        Assert.Throws<BusinessException>(() =>
        {
            _problemManager.Publish(problem);
        });
    }
}
