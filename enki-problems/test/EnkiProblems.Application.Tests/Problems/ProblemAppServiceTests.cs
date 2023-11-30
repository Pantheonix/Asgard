using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asgard.Hermes;
using EnkiProblems.Problems.Tests;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Content;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace EnkiProblems.Problems;

[Collection(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class ProblemAppServiceTests : EnkiProblemsApplicationTestBase
{
    private readonly IProblemAppService _problemAppService;
    private readonly ProblemManager _problemManager;
    private readonly IRepository<Problem, Guid> _problemRepository;
    private readonly EnkiProblemsTestData _testData;
    private ICurrentUser _currentUser;
    private ITestService _testService;

    public ProblemAppServiceTests()
    {
        _problemAppService = GetRequiredService<IProblemAppService>();
        _problemManager = GetRequiredService<ProblemManager>();
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

    #region CreateAsync
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
                    Difficulty = _testData.ProblemDifficulty2
                }
            );
        });
    }
    #endregion

    #region GetListAsync
    [Fact]
    public async Task Should_Not_List_Unpublished_Problems_When_Current_User_Is_Anonymous()
    {
        var problemListDto = await _problemAppService.GetListAsync(new ProblemListFilterDto());

        problemListDto.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Should_List_Published_Problems_When_Current_User_Is_Anonymous()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        var problemListDto = await _problemAppService.GetListAsync(new ProblemListFilterDto());

        problemListDto.TotalCount.ShouldBe(1);
        problemListDto
            .Items
            .ShouldContain(
                p =>
                    p.Name == _testData.ProblemName1
                    && p.ProposerId == _testData.ProposerUserId1
                    && p.IsPublished == true
            );
    }
    #endregion

    #region GetListUnpublishedAsync
    [Fact]
    public async Task Should_List_Unpublished_Problems_Only_For_Current_User_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemListDto = await _problemAppService.GetListUnpublishedAsync();

        problemListDto.TotalCount.ShouldBe(1);
        problemListDto
            .Items
            .ShouldContain(
                p =>
                    p.Name == _testData.ProblemName1
                    && p.ProposerId == _testData.ProposerUserId1
                    && p.IsPublished == false
            );

        problemListDto
            .Items[0]
            .Tests
            .ShouldContain(
                t =>
                    t.Score == _testData.TestScore1
                    && t.InputDownloadUrl == _testData.TestInputLink1
                    && t.OutputDownloadUrl == _testData.TestOutputLink1
            );
    }

    [Fact]
    public async Task Should_Not_List_Unpublished_Problems_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetListUnpublishedAsync();
        });
    }
    #endregion

    #region GetAsync
    [Fact]
    public async Task Should_Get_Published_Problem_When_Current_User_Is_Anonymous()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        var problemDto = await _problemAppService.GetAsync(_testData.ProblemId1);

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
        problemDto.Name.ShouldBe(_testData.ProblemName1);
    }

    [Fact]
    public async Task Should_Not_Get_Unpublished_Problem_When_Current_User_Is_Anonymous()
    {
        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetAsync(_testData.ProblemId1);
        });
    }

    [Fact]
    public async Task Should_Not_Get_Not_Existing_Problem_When_Current_User_Is_Anonymous()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.GetAsync(Guid.NewGuid());
        });
    }
    #endregion

    #region GetUnpublishedAsync
    [Fact]
    public async Task Should_Get_Published_Problem_When_Current_User_Is_Proposer_And_Owner()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.GetUnpublishedAsync(_testData.ProblemId1);

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
        problemDto.Name.ShouldBe(_testData.ProblemName1);
        problemDto
            .Tests
            .ShouldContain(
                t =>
                    t.Score == _testData.TestScore1
                    && t.InputDownloadUrl == _testData.TestInputLink1
                    && t.OutputDownloadUrl == _testData.TestOutputLink1
            );
    }

    [Fact]
    public async Task Should_Get_Unpublished_Problem_When_Current_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.GetUnpublishedAsync(_testData.ProblemId1);

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
        problemDto.Name.ShouldBe(_testData.ProblemName1);
        problemDto
            .Tests
            .ShouldContain(
                t =>
                    t.Score == _testData.TestScore1
                    && t.InputDownloadUrl == _testData.TestInputLink1
                    && t.OutputDownloadUrl == _testData.TestOutputLink1
            );
    }

    [Fact]
    public async Task Should_Not_Get_Unpublished_Problem_When_Current_User_Is_Proposer_And_Not_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetUnpublishedAsync(_testData.ProblemId3);
        });
    }

    [Fact]
    public async Task Should_Not_Get_Not_Existing_Problem_When_Current_User_Is_Proposer()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.GetUnpublishedAsync(Guid.NewGuid());
        });
    }

    [Fact]
    public async Task Should_Not_Get_Published_Problem_When_Current_User_Is_Not_Proposer()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.GetUnpublishedAsync(_testData.ProblemId1);
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
                Difficulty = _testData.ProblemDifficulty2
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
                    Difficulty = _testData.ProblemDifficulty2
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Published_Problem_When_Current_User_Is_Proposer()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<BusinessException>(async () =>
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
                    Difficulty = _testData.ProblemDifficulty2
                }
            );
        });
    }

    [Fact]
    public async Task Should_Update_Published_Problem_When_Current_User_Is_Proposer_And_Owner_And_Problem_Is_About_To_Be_Unpublished()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var problemDto = await _problemAppService.UpdateAsync(
            _testData.ProblemId1,
            new UpdateProblemDto { IsPublished = false }
        );

        problemDto.ShouldNotBeNull();
        problemDto.Id.ShouldBe(_testData.ProblemId1);
        problemDto.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Not_Update_Unpublished_Problem_When_Current_User_Is_Not_Proposer()
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
                    Difficulty = _testData.ProblemDifficulty2
                }
            );
        });
    }
    #endregion

    #region CreateTestAsync
    [Fact]
    public async Task Should_Create_Test_When_Current_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProblemProposerId3, _testData.ProposerUserRoles);

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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful download link retrieval"
                    },
                    InputLink = _testData.TestInputLink1,
                    OutputLink = _testData.TestOutputLink1
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        var problem = await _problemAppService.CreateTestAsync(
            _testData.ProblemId3,
            new CreateTestDto { Score = _testData.TestScore1, ArchiveFile = stubTestArchiveFile }
        );

        problem.ShouldNotBeNull();
        problem.Tests.Count().ShouldBe(1);
        problem.Tests.ShouldContain(t => t.Id == _testData.TestId1);
    }

    [Fact]
    public async Task Should_Not_Create_Test_When_User_Is_Anonymous()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                _testData.ProblemId1,
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Create_Test_When_User_Is_Not_Owner()
    {
        Login(_testData.ProposerUserId2, _testData.ProposerUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                _testData.ProblemId1,
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Create_Test_For_Non_Existing_Problem()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                Guid.NewGuid(),
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Create_Test_For_Published_Problem()
    {
        await PublishProblemAsync(_testData.ProblemId1);

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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful download link retrieval"
                    },
                    InputLink = _testData.TestInputLink1,
                    OutputLink = _testData.TestOutputLink1
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                _testData.ProblemId1,
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Create_Test_When_Upload_Failed()
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

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                _testData.ProblemId1,
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Create_Test_When_Get_Download_Urls_Failed()
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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Failed,
                        Message = "Download link retrieval failed"
                    },
                    InputLink = string.Empty,
                    OutputLink = string.Empty
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.CreateTestAsync(
                _testData.ProblemId1,
                new CreateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }
    #endregion

    #region UpdateTestAsync
    [Fact]
    public async Task Should_Update_Test_When_Current_User_Is_Proposer_And_Owner()
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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful download link retrieval"
                    },
                    InputLink = _testData.TestInputLink1,
                    OutputLink = _testData.TestOutputLink1
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        var problem = await _problemAppService.UpdateTestAsync(
            _testData.ProblemId1,
            _testData.TestId1,
            new UpdateTestDto { Score = _testData.TestScore2, ArchiveFile = stubTestArchiveFile }
        );

        problem.ShouldNotBeNull();
        problem.Tests.Count().ShouldBe(1);
        problem.Tests.ShouldContain(t => t.Id == _testData.TestId1);
        problem.Tests.First().Score.ShouldBe(_testData.TestScore2);
    }

    [Fact]
    public async Task Should_Update_Test_When_Only_Score_Is_Provided()
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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful download link retrieval"
                    },
                    InputLink = _testData.TestInputLink1,
                    OutputLink = _testData.TestOutputLink1
                }
            );

        var problem = await _problemAppService.UpdateTestAsync(
            _testData.ProblemId1,
            _testData.TestId1,
            new UpdateTestDto { Score = _testData.TestScore2 }
        );

        problem.ShouldNotBeNull();
        problem.Tests.Count().ShouldBe(1);
        problem.Tests.ShouldContain(t => t.Id == _testData.TestId1);
        problem.Tests.First().Score.ShouldBe(_testData.TestScore2);
    }

    [Fact]
    public async Task Should_Not_Update_Test_When_User_Is_Anonymous()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                _testData.ProblemId1,
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore2,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Test_When_User_Is_Not_Owner()
    {
        Login(_testData.ProposerUserId2, _testData.ProposerUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                _testData.ProblemId1,
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore2,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Test_For_Non_Existing_Problem()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                Guid.NewGuid(),
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore2,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Test_For_Published_Problem()
    {
        await PublishProblemAsync(_testData.ProblemId1);

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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful download link retrieval"
                    },
                    InputLink = _testData.TestInputLink1,
                    OutputLink = _testData.TestOutputLink1
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                _testData.ProblemId1,
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore1,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

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

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                _testData.ProblemId1,
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore2,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }

    [Fact]
    public async Task Should_Not_Update_Test_When_Get_Download_Urls_Failed()
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

        _testService
            .GetDownloadLinkForTestAsync(Arg.Any<GetDownloadLinkForTestRequest>())
            .Returns(
                new GetDownloadLinkForTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Failed,
                        Message = "Download link retrieval failed"
                    },
                    InputLink = string.Empty,
                    OutputLink = string.Empty
                }
            );

        var stubTestArchiveFile = new RemoteStreamContent(
            new MemoryStream(_testData.TestArchiveBytes1)
        );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.UpdateTestAsync(
                _testData.ProblemId1,
                _testData.TestId1,
                new UpdateTestDto
                {
                    Score = _testData.TestScore2,
                    ArchiveFile = stubTestArchiveFile
                }
            );
        });
    }
    #endregion

    #region DeleteTestAsync
    [Fact]
    public async Task Should_Delete_Test_When_User_Is_Proposer_And_Owner()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        _testService
            .DeleteTestAsync(Arg.Any<DeleteTestRequest>())
            .Returns(
                new DeleteTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Ok,
                        Message = "Successful delete"
                    }
                }
            );

        var problem = await _problemAppService.DeleteTestAsync(
            _testData.ProblemId1,
            _testData.TestId1
        );

        problem.ShouldNotBeNull();
        problem.Tests.ShouldNotContain(t => t.Id == _testData.TestId1);
    }

    [Fact]
    public async Task Should_Not_Delete_Test_When_User_Is_Anonymous()
    {
        Login(_testData.NormalUserId, _testData.NormalUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.DeleteTestAsync(_testData.ProblemId1, _testData.TestId1);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_Test_When_User_Is_Not_Owner()
    {
        Login(_testData.ProposerUserId2, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
        {
            await _problemAppService.DeleteTestAsync(_testData.ProblemId1, _testData.TestId1);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_Test_When_Problem_Is_Not_Found()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.DeleteTestAsync(Guid.NewGuid(), _testData.TestId1);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_Test_When_Problem_Is_Published()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.DeleteTestAsync(_testData.ProblemId1, _testData.TestId1);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_Test_When_Deletion_Failed()
    {
        Login(_testData.ProposerUserId1, _testData.ProposerUserRoles);

        _testService
            .DeleteTestAsync(Arg.Any<DeleteTestRequest>())
            .Returns(
                new DeleteTestResponse
                {
                    Status = new StatusResponse
                    {
                        Code = StatusCode.Failed,
                        Message = "Delete failed"
                    }
                }
            );

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _problemAppService.DeleteTestAsync(_testData.ProblemId1, _testData.TestId1);
        });
    }
    #endregion

    #region GetEvalMetadataAsync
    [Fact]
    public async Task Should_Get_Eval_Metadata_For_Valid_Problem()
    {
        await PublishProblemAsync(_testData.ProblemId1);

        var evalMetadataDto = await _problemAppService.GetEvalMetadataAsync(_testData.ProblemId1);

        evalMetadataDto.ShouldNotBeNull();
        evalMetadataDto.Id.ShouldBe(_testData.ProblemId1);
        evalMetadataDto.Name.ShouldBe(_testData.ProblemName1);
        evalMetadataDto.ProposerId.ShouldBe(_testData.ProblemProposerId1);
        evalMetadataDto.IsPublished.ShouldBeTrue();
        evalMetadataDto.Time.ShouldBe(_testData.ProblemTimeLimit1);
        evalMetadataDto.TotalMemory.ShouldBe(_testData.ProblemTotalMemoryLimit1);
        evalMetadataDto.StackMemory.ShouldBe(_testData.ProblemStackMemoryLimit1);
        evalMetadataDto.IoType.ShouldBe(_testData.ProblemIoType1);
        evalMetadataDto.Tests.Count().ShouldBe(1);
        evalMetadataDto
            .Tests
            .ShouldContain(
                t =>
                    t.Score == _testData.TestScore1
                    && t.InputDownloadUrl == _testData.TestInputLink1
                    && t.OutputDownloadUrl == _testData.TestOutputLink1
            );
    }

    [Fact]
    public async Task Should_Not_Get_Eval_Metadata_For_Non_Existing_Problem()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _problemAppService.GetEvalMetadataAsync(Guid.NewGuid());
        });
    }
    #endregion

    private void Login(Guid userId, string[] userRoles)
    {
        _currentUser.Id.Returns(userId);
        _currentUser.Roles.Returns(userRoles);
        _currentUser.IsAuthenticated.Returns(true);
    }

    private async Task PublishProblemAsync(Guid problemId)
    {
        var problem = await _problemRepository.GetAsync(problemId);
        _problemManager.Publish(problem);
        await _problemRepository.UpdateAsync(problem);
    }
}
