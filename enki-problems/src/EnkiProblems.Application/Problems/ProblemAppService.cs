using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Asgard.Hermes;
using EnkiProblems.Helpers;
using EnkiProblems.Problems.Tests;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace EnkiProblems.Problems;

public class ProblemAppService : EnkiProblemsAppService, IProblemAppService
{
    private readonly ProblemManager _problemManager;
    private readonly IRepository<Problem, Guid> _problemRepository;
    private readonly ITestService _testService;

    public ProblemAppService(
        ProblemManager problemManager,
        IRepository<Problem, Guid> problemRepository,
        ITestService testService
    )
    {
        _problemManager = problemManager;
        _problemRepository = problemRepository;
        _testService = testService;
    }

    [Authorize]
    public async Task<ProblemDto> CreateAsync(CreateProblemDto input)
    {
        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
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

    [AllowAnonymous]
    public async Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input)
    {
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

        problemQueryable = problemQueryable.PageBy(input).OrderBy(p => p.CreationDate);

        var totalCount = await AsyncExecuter.CountAsync(problemQueryable);
        var problems = await AsyncExecuter.ToListAsync(problemQueryable);

        return new PagedResultDto<ProblemDto>(
            totalCount,
            ObjectMapper.Map<List<Problem>, List<ProblemDto>>(problems)
        );
    }

    [Authorize]
    public async Task<PagedResultDto<ProblemDto>> GetUnpublishedProblemsByCurrentUserAsync()
    {
        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        var problemQueryable = await _problemRepository.GetQueryableAsync();

        problemQueryable = problemQueryable
            .Where(p => !p.IsPublished && p.ProposerId == CurrentUser.Id)
            .OrderBy(p => p.CreationDate);

        var totalCount = await AsyncExecuter.CountAsync(problemQueryable);
        var problems = await AsyncExecuter.ToListAsync(problemQueryable);

        return new PagedResultDto<ProblemDto>(
            totalCount,
            ObjectMapper.Map<List<Problem>, List<ProblemDto>>(problems)
        );
    }

    [AllowAnonymous]
    public async Task<ProblemDto> GetByIdAsync(Guid id)
    {
        var problem = await _problemRepository.GetAsync(id);

        if (!problem.IsPublished)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        return ObjectMapper.Map<Problem, ProblemDto>(problem);
    }

    [Authorize]
    public async Task<ProblemDto> GetByIdForProposerAsync(Guid id)
    {
        var problem = await _problemRepository.GetAsync(id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToViewUnpublishedProblems
            );
        }

        if (!problem.IsPublished && problem.ProposerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        return ObjectMapper.Map<Problem, ProblemDto>(problem);
    }

    [Authorize]
    public async Task<ProblemDto> UpdateAsync(Guid id, UpdateProblemDto input)
    {
        var problem = await _problemRepository.GetAsync(id);

        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        if (problem.IsPublished)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
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
            input.Difficulty
        );

        return ObjectMapper.Map<Problem, ProblemDto>(
            await _problemRepository.UpdateAsync(updatedProblem)
        );
    }

    [Authorize]
    public async Task<ProblemWithTestsDto> CreateTestAsync(Guid problemId, CreateTestDto input)
    {
        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(problemId);

        if (problem.IsPublished)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        var testId = problem.Tests.Count + 1;
        var uploadResponse = await _testService.UploadTestAsync(
            new UploadTestStreamDto
            {
                ProblemId = problemId.ToString(),
                TestArchiveBytes = input.ArchiveFile.GetBytes(),
                TestId = testId.ToString()
            }
        );

        if (uploadResponse.Status.Code != StatusCode.Ok)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestUploadFailed,
                $"Test upload failed with status code {uploadResponse.Status.Code}: {uploadResponse.Status.Message}."
            )
                .WithData("problemId", problem.Id)
                .WithData("testId", testId);
        }

        var getDownloadUrlsResponse = await _testService.GetDownloadLinkForTestAsync(
            new GetDownloadLinkForTestRequest
            {
                TestId = testId.ToString(),
                ProblemId = problemId.ToString()
            }
        );

        if (getDownloadUrlsResponse.Status.Code != StatusCode.Ok)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDownloadUrlRetrievalFailed,
                $"Test download url retrieval failed with status code {getDownloadUrlsResponse.Status.Code}: {getDownloadUrlsResponse.Status.Message}."
            )
                .WithData("problemId", problem.Id)
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

    public async Task<ProblemWithTestsDto> UpdateTestAsync(
        Guid problemId,
        int testId,
        UpdateTestDto input
    )
    {
        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(problemId);

        if (problem.IsPublished)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        if (input.ArchiveFile is not null)
        {
            var uploadResponse = await _testService.UploadTestAsync(
                new UploadTestStreamDto
                {
                    ProblemId = problemId.ToString(),
                    TestArchiveBytes = input.ArchiveFile.GetBytes(),
                    TestId = testId.ToString()
                }
            );

            if (uploadResponse.Status.Code != StatusCode.Ok)
            {
                throw new BusinessException(
                    EnkiProblemsDomainErrorCodes.TestUploadFailed,
                    $"Test upload failed with status code {uploadResponse.Status.Code}: {uploadResponse.Status.Message}."
                )
                    .WithData("problemId", problem.Id)
                    .WithData("testId", testId);
            }
        }

        var getDownloadUrlsResponse = await _testService.GetDownloadLinkForTestAsync(
            new GetDownloadLinkForTestRequest
            {
                TestId = testId.ToString(),
                ProblemId = problemId.ToString()
            }
        );

        if (getDownloadUrlsResponse.Status.Code != StatusCode.Ok)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDownloadUrlRetrievalFailed,
                $"Test download url retrieval failed with status code {getDownloadUrlsResponse.Status.Code}: {getDownloadUrlsResponse.Status.Message}."
            )
                .WithData("problemId", problem.Id)
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
    public async Task<ProblemWithTestsDto> DeleteTestAsync(Guid problemId, int testId)
    {
        // TODO: convert to permission
        if (CurrentUser.Roles.All(r => r != EnkiProblemsConsts.ProposerRoleName))
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditProblem
            );
        }

        var problem = await _problemRepository.GetAsync(problemId);

        if (problem.IsPublished)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        var deleteResponse = await _testService.DeleteTestAsync(
            new DeleteTestRequest { ProblemId = problemId.ToString(), TestId = testId.ToString() }
        );

        if (deleteResponse.Status.Code != StatusCode.Ok)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TestDeleteFailed,
                $"Test delete failed with status code {deleteResponse.Status.Code}: {deleteResponse.Status.Message}."
            )
                .WithData("problemId", problem.Id)
                .WithData("testId", testId);
        }

        var updatedProblem = _problemManager.RemoveTest(problem, testId);

        await _problemRepository.UpdateAsync(updatedProblem);

        return ObjectMapper.Map<Problem, ProblemWithTestsDto>(updatedProblem);
    }
}
