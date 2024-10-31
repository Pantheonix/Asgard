using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace EnkiProblems.Problems;

public class ProblemManager : DomainService
{
    private readonly IRepository<Problem, Guid> _problemRepository;
    private readonly ILogger _logger;

    public ProblemManager(
        IRepository<Problem, Guid> problemRepository,
        ILogger<ProblemManager> logger
    )
    {
        _problemRepository = problemRepository;
        _logger = logger;
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
        _logger.LogInformation("Creating problem {Name}", name);

        var problem = await _problemRepository.FirstOrDefaultAsync(p => p.Name == name);
        if (problem is not null)
        {
            _logger.LogError("Problem {Name} already exists", name);
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
        _logger.LogInformation("Updating problem {Name}", problem.Name);

        if (problem.IsPublished && !(isPublished is not null && !isPublished.Value))
        {
            _logger.LogError("Problem {Name} is published and cannot be edited", problem.Name);
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.NotAllowedToEditPublishedProblem
            );
        }

        var oldProblem = await _problemRepository.FirstOrDefaultAsync(p =>
            p.Name == name && p.Id != problem.Id
        );
        if (oldProblem is not null)
        {
            _logger.LogError("Problem {Name} already exists", name);
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
        _logger.LogInformation("Adding test {TestId} to problem {Name}", testId, problem.Name);
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
        _logger.LogInformation("Updating test {TestId} of problem {Name}", testId, problem.Name);
        return problem.UpdateTest(testId, testScore, inputDownloadUrl, outputDownloadUrl);
    }

    public Problem RemoveTest(Problem problem, int testId)
    {
        _logger.LogInformation("Removing test {TestId} from problem {Name}", testId, problem.Name);
        return problem.RemoveTest(testId);
    }

    public Problem Publish(Problem problem)
    {
        _logger.LogInformation("Publishing problem {Name}", problem.Name);
        return problem.Publish();
    }
}
