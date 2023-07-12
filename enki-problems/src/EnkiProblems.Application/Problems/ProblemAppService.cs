using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

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
}
