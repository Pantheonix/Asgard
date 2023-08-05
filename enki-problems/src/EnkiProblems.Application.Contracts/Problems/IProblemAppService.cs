using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EnkiProblems.Problems;

public interface IProblemAppService : IApplicationService
{
    Task<ProblemDto> CreateAsync(CreateProblemDto input);

    Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input);

    Task<PagedResultDto<ProblemWithTestsDto>> GetUnpublishedProblemsByCurrentUserAsync();

    Task<ProblemDto> GetByIdAsync(Guid id);

    Task<ProblemWithTestsDto> GetByIdForProposerAsync(Guid id);

    Task<ProblemDto> UpdateAsync(Guid id, UpdateProblemDto input);

    Task<ProblemWithTestsDto> CreateTestAsync(Guid problemId, CreateTestDto input);

    Task<ProblemWithTestsDto> UpdateTestAsync(Guid problemId, int testId, UpdateTestDto input);

    Task<ProblemWithTestsDto> DeleteTestAsync(Guid problemId, int testId);

    Task<ProblemEvalMetadataDto> GetEvalMetadataAsync(Guid id);
}
