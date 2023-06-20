using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Problem: FullAuditedAggregateRoot<Guid>
{
    public string Name { get; internal set; }
    
    public string Brief { get; internal set; }
    
    public string Description { get; internal set; }
    
    public bool IsPublished { get; private set; }
    
    public Origin Origin { get; internal set; }
    
    public Limit Limit { get; internal set; }
    
    public DateTime CreatedDate { get; internal set; }
    
    public DateTime? PublishedDate { get; internal set; }
    
    public IoTypeEnum IoType { get; internal set; }
    
    public DifficultyEnum Difficulty { get; internal set; }
    
    public int NumberOfTests { get; internal set; }
    
    public ICollection<Test> Tests { get; private set; }

    public ICollection<ProgrammingLanguageEnum> ProgrammingLanguages { get; internal set; }
    
    public ICollection<ProblemLabel> Labels { get; private set; }
    
    private Problem()
    {
    }

    public Problem(
        Guid id,
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
    ): base(id)
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
        
        IsPublished = false;
        CreatedDate = DateTime.Now;

        Tests = new Collection<Test>();
        Labels = new Collection<ProblemLabel>();
    }

    public Problem SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), EnkiProblemsConsts.MaxNameLength, EnkiProblemsConsts.MinNameLength);
        return this;
    }
    
    public Problem SetBrief(string brief)
    {
        Brief = Check.NotNullOrWhiteSpace(brief, nameof(brief), EnkiProblemsConsts.MaxBriefLength, EnkiProblemsConsts.MinBriefLength);
        return this;
    }
    
    public Problem SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(description), EnkiProblemsConsts.MaxDescriptionLength, EnkiProblemsConsts.MinDescriptionLength);
        return this;
    }
    
    public Problem SetOrigin(string sourceName, string authorName)
    {
        Origin = new Origin(Id, sourceName, authorName);
        return this;
    }
    
    public Problem SetLimit(decimal timeLimit, decimal totalMemoryLimit, decimal stackMemoryLimit)
    {
        Limit = new Limit(Id, timeLimit, totalMemoryLimit, stackMemoryLimit);
        return this;
    }

    public Problem SetProgrammingLanguages(IEnumerable<ProgrammingLanguageEnum> programmingLanguageEnums)
    {
        ProgrammingLanguages = new HashSet<ProgrammingLanguageEnum>(programmingLanguageEnums);
        return this;
    }
    
    public Problem SetNumberOfTests(int numberOfTests)
    {
        NumberOfTests = Check.Range(numberOfTests, nameof(numberOfTests), EnkiProblemsConsts.MinNumberOfTests, EnkiProblemsConsts.MaxNumberOfTests);
        return this;
    }
}