using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Problem : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string Brief { get; private set; }

    public string Description { get; private set; }

    public bool IsPublished { get; private set; }

    public Origin Origin { get; private set; }

    public Guid ProposerId { get; private set; }

    public Limit Limit { get; private set; }

    public DateTime CreationDate { get; private set; }

    public DateTime? PublishingDate { get; private set; }

    public IoTypeEnum IoType { get; private set; }

    public DifficultyEnum Difficulty { get; private set; }

    public int NumberOfTests { get; private set; }

    public ICollection<Test> Tests { get; private set; }

    public ICollection<ProgrammingLanguageEnum> ProgrammingLanguages { get; private set; }

    public ICollection<ProblemLabel> Labels { get; private set; }

    private Problem() { }

    internal Problem(
        Guid id,
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
        DifficultyEnum difficulty,
        int numberOfTests,
        IEnumerable<ProgrammingLanguageEnum> programmingLanguages
    )
        : base(id)
    {
        SetName(name);
        SetBrief(brief);
        SetDescription(description);
        SetOrigin(sourceName, authorName);
        SetLimit(timeLimit, totalMemoryLimit, stackMemoryLimit);
        SetProgrammingLanguages(programmingLanguages);
        SetNumberOfTests(numberOfTests);

        IoType = ioType;
        Difficulty = difficulty;
        ProposerId = proposerId;

        IsPublished = false;
        CreationDate = DateTime.Now;

        Tests = new Collection<Test>();
        Labels = new Collection<ProblemLabel>();
    }

    internal Problem SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            EnkiProblemsConsts.MaxNameLength,
            EnkiProblemsConsts.MinNameLength
        );
        return this;
    }

    internal Problem SetBrief(string brief)
    {
        Brief = Check.NotNullOrWhiteSpace(
            brief,
            nameof(brief),
            EnkiProblemsConsts.MaxBriefLength,
            EnkiProblemsConsts.MinBriefLength
        );
        return this;
    }

    internal Problem SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            EnkiProblemsConsts.MaxDescriptionLength,
            EnkiProblemsConsts.MinDescriptionLength
        );
        return this;
    }

    internal Problem SetOrigin(string sourceName, string authorName)
    {
        Origin = new Origin(Id, sourceName, authorName);
        return this;
    }

    internal Problem SetSourceName(string sourceName)
    {
        Origin.SetSourceName(sourceName);
        return this;
    }

    internal Problem SetAuthorName(string authorName)
    {
        Origin.SetAuthorName(authorName);
        return this;
    }

    internal Problem SetLimit(decimal timeLimit, decimal totalMemoryLimit, decimal stackMemoryLimit)
    {
        Limit = new Limit(Id, timeLimit, totalMemoryLimit, stackMemoryLimit);
        return this;
    }

    internal Problem SetTime(decimal timeLimit)
    {
        Limit.SetTime(timeLimit);
        return this;
    }

    internal Problem SetTotalMemory(decimal totalMemory)
    {
        Limit.SetTotalMemory(totalMemory);
        return this;
    }

    internal Problem SetStackMemory(decimal stackMemory)
    {
        Limit.SetStackMemory(stackMemory);
        return this;
    }

    internal Problem SetIoType(IoTypeEnum ioType)
    {
        IoType = ioType;
        return this;
    }

    internal Problem SetDifficulty(DifficultyEnum difficulty)
    {
        Difficulty = difficulty;
        return this;
    }

    internal Problem SetProgrammingLanguages(
        IEnumerable<ProgrammingLanguageEnum> programmingLanguageEnums
    )
    {
        ProgrammingLanguages = new HashSet<ProgrammingLanguageEnum>(programmingLanguageEnums);
        return this;
    }

    internal Problem SetNumberOfTests(int numberOfTests)
    {
        NumberOfTests = Check.Range(
            numberOfTests,
            nameof(numberOfTests),
            EnkiProblemsConsts.MinNumberOfTests,
            EnkiProblemsConsts.MaxNumberOfTests
        );
        return this;
    }
}
