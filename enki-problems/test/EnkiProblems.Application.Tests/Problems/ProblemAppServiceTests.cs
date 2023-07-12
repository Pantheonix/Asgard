using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.Authorization;
using Volo.Abp.Users;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemAppServiceTests : EnkiProblemsApplicationTestBase
{
    private readonly IProblemAppService _problemAppService;
    private readonly EnkiProblemsTestData _testData;
    private ICurrentUser _currentUser;

    public ProblemAppServiceTests()
    {
        _problemAppService = GetRequiredService<IProblemAppService>();
        _testData = GetRequiredService<EnkiProblemsTestData>();
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        _currentUser = Substitute.For<ICurrentUser>();
        services.AddSingleton(_currentUser);
    }

    [Fact]
    public async Task Should_Create_A_New_Valid_Problem_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.CreateAsync(
            new CreateProblemDto
            {
                Name = _testData.ProblemName2,
                Brief = _testData.ProblemBrief2,
                Description = _testData.ProblemDescription2,
                SourceName = _testData.ProblemSourceName2,
                AuthorName = _testData.ProblemAuthorName2,
                Time = _testData.ProblemTimeLimit2,
                StackMemory = _testData.ProblemStackMemoryLimit2,
                TotalMemory = _testData.ProblemTotalMemoryLimit2,
                IoType = _testData.ProblemIoType2,
                Difficulty = _testData.ProblemDifficulty2,
                NumberOfTests = _testData.ProblemNumberOfTests2,
                ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
            }
        );

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldNotBe(Guid.Empty);
        problemDto.Name.ShouldBe(_testData.ProblemName2);
        problemDto.Brief.ShouldBe(_testData.ProblemBrief2);
        problemDto.Description.ShouldBe(_testData.ProblemDescription2);
        problemDto.SourceName.ShouldBe(_testData.ProblemSourceName2);
        problemDto.AuthorName.ShouldBe(_testData.ProblemAuthorName2);
        problemDto.Time.ShouldBe(_testData.ProblemTimeLimit2);
        problemDto.StackMemory.ShouldBe(_testData.ProblemStackMemoryLimit2);
        problemDto.TotalMemory.ShouldBe(_testData.ProblemTotalMemoryLimit2);
        problemDto.IoType.ShouldBe(_testData.ProblemIoType2);
        problemDto.Difficulty.ShouldBe(_testData.ProblemDifficulty2);
        problemDto.NumberOfTests.ShouldBe(_testData.ProblemNumberOfTests2);
        problemDto.ProgrammingLanguages.ShouldBe(_testData.ProblemProgrammingLanguages2);
        problemDto.IsPublished.ShouldBeFalse();
        problemDto.CreationDate.ShouldBeGreaterThan(DateTime.Now.AddMinutes(-1));
    }

    [Fact]
    public async Task Should_Not_Create_A_New_Valid_Problem_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.CreateAsync(
                new CreateProblemDto
                {
                    Name = _testData.ProblemName2,
                    Brief = _testData.ProblemBrief2,
                    Description = _testData.ProblemDescription2,
                    SourceName = _testData.ProblemSourceName2,
                    AuthorName = _testData.ProblemAuthorName2,
                    Time = _testData.ProblemTimeLimit2,
                    StackMemory = _testData.ProblemStackMemoryLimit2,
                    TotalMemory = _testData.ProblemTotalMemoryLimit2,
                    IoType = _testData.ProblemIoType2,
                    Difficulty = _testData.ProblemDifficulty2,
                    NumberOfTests = _testData.ProblemNumberOfTests2,
                    ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
                }
            );
        });
    }

    private void Login(Guid userId, string[] userRoles)
    {
        _currentUser.Id.Returns(userId);
        _currentUser.Roles.Returns(userRoles);
        _currentUser.IsAuthenticated.Returns(true);
    }
}
