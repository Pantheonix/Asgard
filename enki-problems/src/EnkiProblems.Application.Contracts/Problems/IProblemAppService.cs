using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EnkiProblems.Problems;

public interface IProblemAppService : IApplicationService
{
    Task<ProblemDto> CreateAsync(CreateProblemDto input);

    Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input);

    Task<PagedResultDto<ProblemDto>> GetUnpublishedProblemsByCurrentUserAsync();

    Task<ProblemDto> GetByIdAsync(Guid id);

    Task<ProblemDto> GetByIdForProposerAsync(Guid id);

    Task<ProblemDto> UpdateAsync(Guid id, UpdateProblemDto input);
}
