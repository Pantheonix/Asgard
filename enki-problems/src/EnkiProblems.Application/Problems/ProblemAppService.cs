using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Entities;

namespace EnkiProblems.Problems;

public class ProblemAppService : EnkiProblemsAppService, IProblemAppService
{
    private readonly ProblemManager _problemManager;
    private readonly IRepository<Problem, Guid> _problemRepository;

    public ProblemAppService(
        ProblemManager problemManager,
        IRepository<Problem, Guid> problemRepository
    )
    {
        _problemManager = problemManager;
        _problemRepository = problemRepository;
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
            input.Difficulty,
            input.NumberOfTests,
            input.ProgrammingLanguages
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

        if (input.ProgrammingLanguages is not null)
        {
            problemQueryable = problemQueryable.Where(
                p => p.ProgrammingLanguages.Any(l => input.ProgrammingLanguages.Contains(l))
            );
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

        if (problem is null)
        {
            throw new EntityNotFoundException(typeof(Problem), id);
        }

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

        if (problem is null)
        {
            throw new EntityNotFoundException(typeof(Problem), id);
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

        if (problem is null)
        {
            throw new EntityNotFoundException(typeof(Problem), id);
        }

        if (problem.IsPublished)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        if (problem.ProposerId != CurrentUser.Id)
        {
            throw new AbpAuthorizationException(
                EnkiProblemsDomainErrorCodes.UnpublishedProblemNotBelongingToCurrentUser
            );
        }

        await _problemManager.UpdateAsync(
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
            input.NumberOfTests,
            input.ProgrammingLanguages
        );

        return ObjectMapper.Map<Problem, ProblemDto>(await _problemRepository.UpdateAsync(problem));
    }
}
