using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace EnkiProblems.Problems;

public class ProblemManager : DomainService
{
    private readonly IRepository<Problem, Guid> _problemRepository;

    public ProblemManager(IRepository<Problem, Guid> problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<Problem> CreateAsync(
        string name,
        string brief,
        string description,
        string sourceName,
        string authorName,
        Guid proposerId,
        decimal timeLimit,
        decimal totalMemoryLimit,
        decimal stackMemoryLimit,
        IoTypeEnum ioType,
        DifficultyEnum difficulty
    )
    {
        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Name == name);
        if (problem is not null)
        {
            throw new BusinessException(EnkiProblemsDomainErrorCodes.ProblemNameAlreadyExists);
        }

        return new Problem(
            GuidGenerator.Create(),
            name,
            brief,
            description,
            sourceName,
            authorName,
            proposerId,
            timeLimit,
            totalMemoryLimit,
            stackMemoryLimit,
            ioType,
            difficulty
        );
    }

    public async Task<Problem> UpdateAsync(
        Problem problem,
        string? name,
        string? brief,
        string? description,
        string? sourceName,
        string? authorName,
        decimal? timeLimit,
        decimal? totalMemoryLimit,
        decimal? stackMemoryLimit,
        IoTypeEnum? ioType,
        DifficultyEnum? difficulty,
        bool? isPublished
    )
    {
        if (problem.IsPublished && !(isPublished is not null && !isPublished.Value))
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        var oldProblem = await _problemRepository.FirstOrDefaultAsync(p => p.Name == name);
        if (oldProblem is not null)
        {
            throw new BusinessException(EnkiProblemsDomainErrorCodes.ProblemNameAlreadyExists);
        }

        if (name is not null)
        {
            problem.SetName(name);
        }

        if (brief is not null)
        {
            problem.SetBrief(brief);
        }

        if (description is not null)
        {
            problem.SetDescription(description);
        }

        if (sourceName is not null)
        {
            problem.SetSourceName(sourceName);
        }

        if (authorName is not null)
        {
            problem.SetAuthorName(authorName);
        }

        if (timeLimit is not null)
        {
            problem.SetTime((decimal)timeLimit);
        }

        if (totalMemoryLimit is not null)
        {
            problem.SetTotalMemory((decimal)totalMemoryLimit);
        }

        if (stackMemoryLimit is not null)
        {
            problem.SetStackMemory((decimal)stackMemoryLimit);
        }

        if (ioType is not null)
        {
            problem.SetIoType((IoTypeEnum)ioType);
        }

        if (difficulty is not null)
        {
            problem.SetDifficulty((DifficultyEnum)difficulty);
        }

        switch (isPublished)
        {
            case null:
                return problem;
            case true:
                problem.Publish();
                break;
            default:
                problem.Unpublish();
                break;
        }

        return problem;
    }

    public Problem AddTest(
        Problem problem,
        int testId,
        int testScore,
        string inputDownloadUrl,
        string outputDownloadUrl
    )
    {
        return problem.AddTest(testId, testScore, inputDownloadUrl, outputDownloadUrl);
    }

    public Problem UpdateTest(
        Problem problem,
        int testId,
        int? testScore,
        string? inputDownloadUrl,
        string? outputDownloadUrl
    )
    {
        return problem.UpdateTest(testId, testScore, inputDownloadUrl, outputDownloadUrl);
    }

    public Problem RemoveTest(Problem problem, int testId)
    {
        return problem.RemoveTest(testId);
    }

    public Problem Publish(Problem problem)
    {
        return problem.Publish();
    }
}
