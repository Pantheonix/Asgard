using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asgard.Hermes;
using EnkiProblems.Problems.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace EnkiProblems.Problems;

public class ProblemAppService : EnkiProblemsAppService, IProblemAppService
{
    private readonly ProblemManager _problemManager;
    private readonly IRepository<Problem, Guid> _problemRepository;
    private readonly ITestService _testService;
    private readonly ILogger _logger;

    public ProblemAppService(
        ProblemManager problemManager,
        IRepository<Problem, Guid> problemRepository,
        ITestService testService,
        ILogger<ProblemAppService> logger
    )
    {
        _problemManager = problemManager;
        _problemRepository = problemRepository;
        _testService = testService;
        _logger = logger;
    }

    [Authorize]
    public async Task<ProblemDto> CreateAsync(CreateProblemDto input)
    {
        _logger.LogInformation("Creating problem {Name}", input.Name);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to create problems", CurrentUser.Id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToCreateProblem
            );
        }

        var problem = await _problemManager.CreateAsync(
            input.Name,
            input.Brief,
            input.Description,
            input.SourceName,
            input.AuthorName,
            (Guid)CurrentUser.Id,
            input.Time,
            input.TotalMemory,
            input.StackMemory,
            input.IoType,
            input.Difficulty
        );

        await _problemRepository.InsertAsync(problem);

        return ObjectMapper.Map<Problem, ProblemDto>(problem);
    }

    [Authorize]
    public async Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input)
    {
        _logger.LogInformation("Getting problems list");

        var problemQueryable = await _problemRepository.GetQueryableAsync();

        problemQueryable = problemQueryable.Where(p => p.IsPublished);

        if (!string.IsNullOrEmpty(input.Name))
        {
            problemQueryable = problemQueryable.Where(p => p.Name.Contains(input.Name));
        }

        if (input.ProposerId is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.ProposerId == input.ProposerId);
        }

        if (input.IoType is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.IoType == input.IoType);
        }

        if (input.Difficulty is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.Difficulty == input.Difficulty);
        }

        var totalCount = await AsyncExecuter.CountAsync(problemQueryable);
        problemQueryable = problemQueryable.PageBy(input).OrderBy(p => p.CreationDate);

        var problems = await AsyncExecuter.ToListAsync(problemQueryable);

        return new PagedResultDto<ProblemDto>(
            totalCount,
            ObjectMapper.Map<List<Problem>, List<ProblemDto>>(problems)
        );
    }

    [Authorize]
    public async Task<PagedResultDto<ProblemWithTestsDto>> GetListUnpublishedAsync(ProblemListFilterDto input)
    {
        _logger.LogInformation(
            "Getting unpublished problems list for user {UserId}",
            CurrentUser.Id
        );

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError(
                "User {UserId} is not allowed to view unpublished problems",
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        var problemQueryable = await _problemRepository.GetQueryableAsync();

        problemQueryable = problemQueryable
            .Where(p => !p.IsPublished && p.ProposerId == CurrentUser.Id);

        if (!string.IsNullOrEmpty(input.Name))
        {
            problemQueryable = problemQueryable.Where(p => p.Name.Contains(input.Name));
        }

        if (input.ProposerId is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.ProposerId == input.ProposerId);
        }

        if (input.IoType is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.IoType == input.IoType);
        }

        if (input.Difficulty is not null)
        {
            problemQueryable = problemQueryable.Where(p => p.Difficulty == input.Difficulty);
        }

        var totalCount = await AsyncExecuter.CountAsync(problemQueryable);
        problemQueryable = problemQueryable.PageBy(input).OrderBy(p => p.CreationDate);

        var problems = await AsyncExecuter.ToListAsync(problemQueryable);

        return new PagedResultDto<ProblemWithTestsDto>(
            totalCount,
            ObjectMapper.Map<List<Problem>, List<ProblemWithTestsDto>>(problems)
        );
    }

    [Authorize]
    public async Task<ProblemDto> GetAsync(Guid id)
    {
        _logger.LogInformation("Getting problem {ProblemId}", id);

        var problem = await _problemRepository.GetAsync(id);

        if (!problem.IsPublished)
        {
            _logger.LogError("Problem {ProblemId} is not published", id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        return ObjectMapper.Map<Problem, ProblemDto>(problem);
    }

    [Authorize]
    public async Task<ProblemWithTestsDto> GetUnpublishedAsync(Guid id)
    {
        _logger.LogInformation("Getting problem {ProblemId} for user {UserId}", id, CurrentUser.Id);

        var problem = await _problemRepository.GetAsync(id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError(
                "User {UserId} is not allowed to view unpublished problems",
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        if (!problem.IsPublished && problem.ProposerId != CurrentUser.Id)
        {
            _logger.LogError(
                "Problem {ProblemId} is not published and does not belong to user {UserId}",
                id,
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        return ObjectMapper.Map<Problem, ProblemWithTestsDto>(problem);
    }

    [Authorize]
    public async Task<ProblemDto> UpdateAsync(Guid id, UpdateProblemDto input)
    {
        _logger.LogInformation("Updating problem {ProblemId}", id);

        var problem = await _problemRepository.GetAsync(id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to edit problems", CurrentUser.Id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            _logger.LogError(
                "Problem {ProblemId} does not belong to user {UserId}",
                id,
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        var updatedProblem = await _problemManager.UpdateAsync(
            problem,
            input.Name,
            input.Brief,
            input.Description,
            input.SourceName,
            input.AuthorName,
            input.Time,
            input.TotalMemory,
            input.StackMemory,
            input.IoType,
            input.Difficulty,
            input.IsPublished
        );

        return ObjectMapper.Map<Problem, ProblemDto>(
            await _problemRepository.UpdateAsync(updatedProblem)
        );
    }

    [Authorize]
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting problem {ProblemId}", id);

        var problem = await _problemRepository.GetAsync(id);

        if (problem is null)
        {
            _logger.LogError("Problem {ProblemId} not found", id);
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.ProblemNotFound,
                $"Problem {id} not found."
            );
        }

        // TODO: convert to permission
        if (problem.IsPublished && CurrentUser.Roles.All(r => r != EnkiProblemsConsts.AdminRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to delete problem {ProblemId}", CurrentUser.Id, id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToDeletePublishedProblem
            );
        }

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.AdminRoleName) &&
            CurrentUser.Id != id)
        {
            _logger.LogError("User {UserId} is not allowed to delete problem {ProblemId}", CurrentUser.Id, id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.ProblemCannotBeDeleted
            );
        }

        var problemTests = problem.Tests.ToList();
        
        foreach (var problemTest in problemTests)
        {
            var deleteResponse = await _testService.DeleteTestAsync(
                new DeleteTestRequest { ProblemId = id.ToString(), TestId = problemTest.Id.ToString() }
            );
        
            if (deleteResponse.Status.Code != StatusCode.Ok)
            {
                _logger.LogError(
                    "Test delete failed with status code {StatusCode}: {StatusMessage}",
                    deleteResponse.Status.Code,
                    deleteResponse.Status.Message
                );
                throw new BusinessException(
                        EnkiProblemsDomainErrorCodes.TestDeleteFailed,
                        $"Test delete failed with status code {deleteResponse.Status.Code}: {deleteResponse.Status.Message}."
                    )
                    .WithData("id", problem.Id)
                    .WithData("testId", problemTest.Id);
            }

            problem = _problemManager.RemoveTest(problem, problemTest.Id);
        }

        await _problemRepository.DeleteAsync(problem);
    }

    [Authorize]
    public async Task<ProblemWithTestsDto> CreateTestAsync(Guid id, CreateTestDto input)
    {
        _logger.LogInformation("Creating test for problem {ProblemId}", id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to edit problems", CurrentUser.Id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(id);

        if (problem.IsPublished)
        {
            _logger.LogError("Problem {ProblemId} is published and cannot be edited", id);
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            _logger.LogError(
                "Problem {ProblemId} does not belong to user {UserId}",
                id,
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        var testId = problem.GetFirstAvailableTestId();
        var uploadResponse = await _testService.UploadTestAsync(
            new UploadTestStreamDto
            {
                ProblemId = id.ToString(),
                TestArchiveBytes = await input.ArchiveFile.GetStream().GetAllBytesAsync(),
                TestId = testId.ToString()
            }
        );

        if (uploadResponse.Status.Code != StatusCode.Ok)
        {
            _logger.LogError(
                "Test upload failed with status code {StatusCode}: {StatusMessage}",
                uploadResponse.Status.Code,
                uploadResponse.Status.Message
            );
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestUploadFailed,
                $"Test upload failed with status code {uploadResponse.Status.Code}: {uploadResponse.Status.Message}."
            )
                .WithData("id", problem.Id)
                .WithData("testId", testId);
        }

        var getDownloadUrlsResponse = await _testService.GetDownloadLinkForTestAsync(
            new GetDownloadLinkForTestRequest
            {
                TestId = testId.ToString(),
                ProblemId = id.ToString()
            }
        );

        if (getDownloadUrlsResponse.Status.Code != StatusCode.Ok)
        {
            _logger.LogError(
                "Test download url retrieval failed with status code {StatusCode}: {StatusMessage}",
                getDownloadUrlsResponse.Status.Code,
                getDownloadUrlsResponse.Status.Message
            );
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDownloadUrlRetrievalFailed,
                $"Test download url retrieval failed with status code {getDownloadUrlsResponse.Status.Code}: {getDownloadUrlsResponse.Status.Message}."
            )
                .WithData("id", problem.Id)
                .WithData("testId", testId);
        }

        var updatedProblem = _problemManager.AddTest(
            problem,
            testId,
            input.Score,
            getDownloadUrlsResponse.InputLink,
            getDownloadUrlsResponse.OutputLink
        );
        await _problemRepository.UpdateAsync(updatedProblem);

        return ObjectMapper.Map<Problem, ProblemWithTestsDto>(updatedProblem);
    }

    [Authorize]
    public async Task<ProblemWithTestsDto> UpdateTestAsync(Guid id, int testId, UpdateTestDto input)
    {
        _logger.LogInformation("Updating test {TestId} for problem {ProblemId}", testId, id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to edit problems", CurrentUser.Id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(id);

        if (problem.IsPublished)
        {
            _logger.LogError("Problem {ProblemId} is published and cannot be edited", id);
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            _logger.LogError(
                "Problem {ProblemId} does not belong to user {UserId}",
                id,
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        if (input.ArchiveFile is not null)
        {
            var uploadResponse = await _testService.UploadTestAsync(
                new UploadTestStreamDto
                {
                    ProblemId = id.ToString(),
                    TestArchiveBytes = await input.ArchiveFile.GetStream().GetAllBytesAsync(),
                    TestId = testId.ToString()
                }
            );

            if (uploadResponse.Status.Code != StatusCode.Ok)
            {
                _logger.LogError(
                    "Test upload failed with status code {StatusCode}: {StatusMessage}",
                    uploadResponse.Status.Code,
                    uploadResponse.Status.Message
                );
                throw new BusinessException(
                    EnkiProblemsDomainErrorCodes.TestUploadFailed,
                    $"Test upload failed with status code {uploadResponse.Status.Code}: {uploadResponse.Status.Message}."
                )
                    .WithData("id", problem.Id)
                    .WithData("testId", testId);
            }
        }

        var getDownloadUrlsResponse = await _testService.GetDownloadLinkForTestAsync(
            new GetDownloadLinkForTestRequest
            {
                TestId = testId.ToString(),
                ProblemId = id.ToString()
            }
        );

        if (getDownloadUrlsResponse.Status.Code != StatusCode.Ok)
        {
            _logger.LogError(
                "Test download url retrieval failed with status code {StatusCode}: {StatusMessage}",
                getDownloadUrlsResponse.Status.Code,
                getDownloadUrlsResponse.Status.Message
            );
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDownloadUrlRetrievalFailed,
                $"Test download url retrieval failed with status code {getDownloadUrlsResponse.Status.Code}: {getDownloadUrlsResponse.Status.Message}."
            )
                .WithData("id", problem.Id)
                .WithData("testId", testId);
        }

        _problemManager.UpdateTest(
            problem,
            testId,
            input.Score,
            getDownloadUrlsResponse.InputLink,
            getDownloadUrlsResponse.OutputLink
        );

        await _problemRepository.UpdateAsync(problem);

        return ObjectMapper.Map<Problem, ProblemWithTestsDto>(problem);
    }

    [Authorize]
    public async Task<ProblemWithTestsDto> DeleteTestAsync(Guid id, int testId)
    {
        _logger.LogInformation("Deleting test {TestId} for problem {ProblemId}", testId, id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            _logger.LogError("User {UserId} is not allowed to edit problems", CurrentUser.Id);
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(id);

        if (problem.IsPublished)
        {
            _logger.LogError("Problem {ProblemId} is published and cannot be edited", id);
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            _logger.LogError(
                "Problem {ProblemId} does not belong to user {UserId}",
                id,
                CurrentUser.Id
            );
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        var deleteResponse = await _testService.DeleteTestAsync(
            new DeleteTestRequest { ProblemId = id.ToString(), TestId = testId.ToString() }
        );

        if (deleteResponse.Status.Code != StatusCode.Ok)
        {
            _logger.LogError(
                "Test delete failed with status code {StatusCode}: {StatusMessage}",
                deleteResponse.Status.Code,
                deleteResponse.Status.Message
            );
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDeleteFailed,
                $"Test delete failed with status code {deleteResponse.Status.Code}: {deleteResponse.Status.Message}."
            )
                .WithData("id", problem.Id)
                .WithData("testId", testId);
        }

        var updatedProblem = _problemManager.RemoveTest(problem, testId);

        await _problemRepository.UpdateAsync(updatedProblem);

        return ObjectMapper.Map<Problem, ProblemWithTestsDto>(updatedProblem);
    }

    public async Task<ProblemEvalMetadataDto> GetEvalMetadataAsync(Guid id)
    {
        _logger.LogInformation("Getting eval metadata for problem {ProblemId}", id);

        var problem = await _problemRepository.GetAsync(id);

        return ObjectMapper.Map<Problem, ProblemEvalMetadataDto>(problem);
    }
}
