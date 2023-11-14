using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EnkiProblems.Problems;

public interface IProblemAppService : IApplicationService
{
    Task<ProblemDto> CreateAsync(CreateProblemDto input);

    Task<PagedResultDto<ProblemDto>> GetListAsync(ProblemListFilterDto input);

    Task<PagedResultDto<ProblemWithTestsDto>> GetListUnpublishedAsync();

    Task<ProblemDto> GetAsync(Guid id);

    Task<ProblemWithTestsDto> GetUnpublishedAsync(Guid id);

    Task<ProblemDto> UpdateAsync(Guid id, UpdateProblemDto input);

    Task<ProblemWithTestsDto> CreateTestAsync(Guid id, CreateTestDto input);

    Task<ProblemWithTestsDto> UpdateTestAsync(Guid id, int testId, UpdateTestDto input);

    Task<ProblemWithTestsDto> DeleteTestAsync(Guid id, int testId);

    Task<ProblemEvalMetadataDto> GetEvalMetadataAsync(Guid id);
}
