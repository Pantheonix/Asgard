using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asgard.Hermes;
using EnkiProblems.Problems.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemAppServiceTests : EnkiProblemsApplicationTestBase
{
    private readonly IProblemAppService _problemAppService;
    private readonly IRepository<Problem, Guid> _problemRepository;
    private readonly EnkiProblemsTestData _testData;
    private ICurrentUser _currentUser;
    private ITestService _testService;

    public ProblemAppServiceTests()
    {
        _problemAppService = GetRequiredService<IProblemAppService>();
        _problemRepository = GetRequiredService<IRepository<Problem, Guid>>();
        _testData = GetRequiredService<EnkiProblemsTestData>();
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        _currentUser = Substitute.For<ICurrentUser>();
        services.AddSingleton(_currentUser);

        _testService = Substitute.For<ITestService>();
        services.AddSingleton(_testService);
    }

    #region CreateAsyncTests
    [Fact]
    public async Task Should_Create_A_New_Valid_Problem_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

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
        problemDto.NumberOfTests.ShouldBe(0);
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
                    ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
                }
            );
        });
    }
    #endregion

    #region GetListAsyncTests
    [Fact]
    public async Task Should_Not_List_Unpublished_Problems_When_Current_User_Is_Anonymous()
    {
        var problemListDto = await _problemAppService.GetListAsync(new ProblemListFilterDto());

        problemListDto.TotalCount.ShouldBe(0);
    }
    #endregion

    #region GetUnpublishedProblemsByCurrentUserAsyncTests
    [Fact]
    public async Task Should_List_Unpublished_Problems_Only_For_Current_User_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemListDto = await _problemAppService.GetUnpublishedProblemsByCurrentUserAsync();

        problemListDto.TotalCount.ShouldBe(1);
        problemListDto.Items.ShouldContain(
            p =>
                p.Name == _testData.ProblemName1
                && p.ProposerId == _testData.ProposerUserId1
                && p.IsPublished == false
        );
    }

    [Fact]
    public async Task Should_Not_List_Unpublished_Problems_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetUnpublishedProblemsByCurrentUserAsync();
        });
    }
    #endregion

    #region GetByIdAsyncTests
    // TODO: Run this test after implementing the PublishAsync method support
    // [Fact]
    // public async Task Should_Get_Published_Problem_When_Current_User_Is_Anonymous()
    // {
    //     var problemDto = await _problemAppService.GetByIdAsync(new GetProblemByIdDto
    //     {
    //         ProblemId = _testData.ProblemId1
    //     });
    //
    //     problemDto.ShouldNotBeNull();
    //     problemDto.Id.ShouldBe(_testData.ProblemId1);
    //     problemDto.Name.ShouldBe(_testData.ProblemName1);
    // }

    [Fact]
    public async Task Should_Not_Get_Unpublished_Problem_When_Current_User_Is_Anonymous()
    {
        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetByIdAsync(_testData.ProblemId1);
        });
    }

    [Fact]
    public async Task Should_Not_Get_Not_Existing_Problem_When_Current_User_Is_Anonymous()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.GetByIdAsync(Guid.NewGuid());
        });
    }
    #endregion

    #region GetByIdForProposerAsyncTests
    // TODO: Run this test after implementing the PublishAsync method support
    // [Fact]
    // public async Task Should_Get_Published_Problem_When_Current_User_Is_Proposer()
    // {
    //     Login(_testData.ProposerUserId, _testData.ProposerUserRoles);
    //
    //     var problemDto = await _problemAppService.GetByIdForProposerAsync(new GetProblemByIdDto
    //     {
    //         ProblemId = _testData.ProblemId1
    //     });
    //
    //     problemDto.ShouldNotBeNull();
    //     problemDto.Id.ShouldBe(_testData.ProblemId1);
    //     problemDto.Name.ShouldBe(_testData.ProblemName1);
    // }

    [Fact]
    public async Task Should_Get_Unpublished_Problem_When_Current_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.GetByIdForProposerAsync(_testData.ProblemId1);

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
        problemDto.Name.ShouldBe(_testData.ProblemName1);
    }

    [Fact]
    public async Task Should_Not_Get_Unpublished_Problem_When_Current_User_Is_Proposer_And_Not_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetByIdForProposerAsync(_testData.ProblemId3);
        });
    }

    [Fact]
    public async Task Should_Not_Get_Not_Existing_Problem_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.GetByIdForProposerAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Not_Get_Published_Problem_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetByIdForProposerAsync(_testData.ProblemId1);
        });
    }
    #endregion

    #region UpdateAsync
    [Fact]
    public async Task Should_Update_Unpublished_Problem_When_Current_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.UpdateAsync(
            _testData.ProblemId1,
            new UpdateProblemDto
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
                ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
            }
        );

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
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
        problemDto.NumberOfTests.ShouldBe(_testData.ProblemNumberOfTests1);
        problemDto.ProgrammingLanguages.ShouldBe(_testData.ProblemProgrammingLanguages2);
        problemDto.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Not_Update_Unpublished_Problem_When_Current_User_Is_Proposer_And_Not_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.UpdateAsync(
                _testData.ProblemId3,
                new UpdateProblemDto
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
                    ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
                }
            );
        });
    }

    // TODO: Run this test after implementing the PublishAsync method support
    // [Fact]
    // public async Task Should_Not_Update_Published_Problem_When_Current_User_Is_Proposer()
    // {
    //     Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

    //     await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
    //     {
    //         await _problemAppService.UpdateAsync(
    //             _testData.ProblemId1,
    //             new UpdateProblemDto
    //             {
    //                 Name = _testData.ProblemName2,
    //                 Brief = _testData.ProblemBrief2,
    //                 Description = _testData.ProblemDescription2,
    //                 SourceName = _testData.ProblemSourceName2,
    //                 AuthorName = _testData.ProblemAuthorName2,
    //                 Time = _testData.ProblemTimeLimit2,
    //                 StackMemory = _testData.ProblemStackMemoryLimit2,
    //                 TotalMemory = _testData.ProblemTotalMemoryLimit2,
    //                 IoType = _testData.ProblemIoType2,
    //                 Difficulty = _testData.ProblemDifficulty2,
    //                 NumberOfTests = _testData.ProblemNumberOfTests2,
    //                 ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
    //             }
    //         );
    //     });
    // }

    [Fact]
    public async Task Should_Not_Update_Problem_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.UpdateAsync(
                _testData.ProblemId1,
                new UpdateProblemDto
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
                    ProgrammingLanguages = _testData.ProblemProgrammingLanguages2
                }
            );
        });
    }
    #endregion

    #region AddTestAsync
    [Fact]
    public async Task Should_Add_Test_When_Current_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        _testService
            .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
            .Returns(
                new UploadResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful upload"
                    }
                }
            );

        var stubTestArchiveFile = new FormFile(
            new MemoryStream(_testData.TestArchiveBytes1),
            0,
            0,
            "Test archive file",
            "Test archive file"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };

        var problem = await _problemAppService.AddTestAsync(
            _testData.ProblemId1,
            new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
        );

        problem.ShouldNotBeNull();
        problem.Tests.Count().ShouldBe(2);
        problem.Tests.ShouldContain(t => t.Id == _testData.TestId1);
    }

    [Fact]
    public async Task Should_Not_Add_Test_When_User_Is_Anonymous()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        _testService
            .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
            .Returns(
                new UploadResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful upload"
                    }
                }
            );

        var stubTestArchiveFile = new FormFile(
            new MemoryStream(_testData.TestArchiveBytes1),
            0,
            0,
            "Test archive file",
            "Test archive file"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.AddTestAsync(
                _testData.ProblemId1,
                new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Add_Test_When_User_Is_Not_Owner()
    {
        Login(_testData.ProposerUserId2, _testData.ProposerUserRoles);

        _testService
            .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
            .Returns(
                new UploadResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful upload"
                    }
                }
            );

        var stubTestArchiveFile = new FormFile(
            new MemoryStream(_testData.TestArchiveBytes1),
            0,
            0,
            "Test archive file",
            "Test archive file"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.AddTestAsync(
                _testData.ProblemId1,
                new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Add_Test_For_Non_Existing_Problem()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        _testService
            .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
            .Returns(
                new UploadResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful upload"
                    }
                }
            );

        var stubTestArchiveFile = new FormFile(
            new MemoryStream(_testData.TestArchiveBytes1),
            0,
            0,
            "Test archive file",
            "Test archive file"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.AddTestAsync(
                Guid.NewGuid(),
                new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
            );
        });
    }

    // TODO: run test when problem publishing is implemented
    // [Fact]
    // public async Task Should_Not_Add_Test_For_Published_Problem()
    // {
    //     Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);
    //
    //      _testService
    //                 .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
    //                 .Returns(
    //                     new UploadResponse
    //                     {
    //                         Status = new StatusResponse
    //                         {
    //                             Code = StatusCode.Ok,
    //                             Message = "Successful upload"
    //                         }
    //                     }
    //                 );
    //
    //             var stubTestArchiveFile = new FormFile(
    //                 new MemoryStream(_testData.TestArchiveBytes1),
    //                 0,
    //                 0,
    //                 "Test archive file",
    //                 "Test archive file"
    //             )
    //             {
    //                 Headers = new HeaderDictionary(),
    //                 ContentType = "application/zip"
    //             };
    //
    //             // TODO: Publish problem
    //
    //     await Assert.ThrowsAsync<Business>(async () =>
    //     {
    //         await _problemAppService.AddTestAsync(
    //             _testData.ProblemId3,
    //             new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
    //         );
    //     });
    // }

    [Fact]
    public async Task Should_Not_Add_Test_When_Upload_Failed()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        _testService
            .UploadTestAsync(Arg.Any<UploadTestStreamDto>())
            .Returns(
                new UploadResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Failed,
                        Message = "Upload failed"
                    }
                }
            );

        var stubTestArchiveFile = new FormFile(
            new MemoryStream(_testData.TestArchiveBytes1),
            0,
            0,
            "Test archive file",
            "Test archive file"
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.AddTestAsync(
                _testData.ProblemId1,
                new AddTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
            );
        });
    }
    #endregion

    private void Login(Guid userId, string[] userRoles)
    {
        _currentUser.Id.Returns(userId);
        _currentUser.Roles.Returns(userRoles);
        _currentUser.IsAuthenticated.Returns(true);
    }
}
