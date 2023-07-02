using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace EnkiProblems.Problems;

public class ProblemsManager : DomainService
{
    private readonly IRepository<Problem, Guid> _problemRepository;

    public ProblemsManager(IRepository<Problem, Guid> problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<Problem> CreateAsync(
        string name,
        string brief,
        string description,
        string sourceName,
        string authorName,
        decimal timeLimit,
        decimal totalMemoryLimit,
        decimal stackMemoryLimit,
        IoTypeEnum ioType,
        DifficultyEnum difficulty,
        int numberOfTests,
        IEnumerable<ProgrammingLanguageEnum> programmingLanguages
    )
    {
        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Name == name);
        if (problem is not null)
        {
            throw new BusinessException(EnkiProblemsDomainErrorCodes.ProblemAlreadyExists);
        }

        return new Problem(
            GuidGenerator.Create(),
            name,
            brief,
            description,
            sourceName,
            authorName,
            timeLimit,
            totalMemoryLimit,
            stackMemoryLimit,
            ioType,
            difficulty,
            numberOfTests,
            programmingLanguages
        );
    }
}
