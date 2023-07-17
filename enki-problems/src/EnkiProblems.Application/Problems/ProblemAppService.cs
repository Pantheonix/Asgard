using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;

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

    public async Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input)
    {
        var problemQueryable = await _problemRepository.GetQueryableAsync();

        problemQueryable = problemQueryable.Where(p => p.IsPublished);

        if (!string.IsNullOrEmpty(input.Name))
        {
            problemQueryable = problemQueryable.Where(p => p.Name.Contains(input.Name));
        }

        if (input.IoType != null)
        {
            problemQueryable = problemQueryable.Where(p => p.IoType == input.IoType);
        }

        if (input.Difficulty != null)
        {
            problemQueryable = problemQueryable.Where(p => p.Difficulty == input.Difficulty);
        }

        if (input.ProgrammingLanguages != null)
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
}
