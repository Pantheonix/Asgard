using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace EnkiProblems.Problems;

public class Origin: ValueObject
{
    public Guid ProblemId { get; private set; }
    
    public string SourceName { get; private set; }
    
    public string AuthorName { get; private set; }
    
    private Origin()
    {
    }
    
    internal Origin(
        Guid problemId,
        string sourceName,
        string authorName
    )
    {
        ProblemId = problemId;
        SetSourceName(sourceName);
        SetAuthorName(authorName);
    }

    public Origin SetSourceName(string sourceName)
    {
        SourceName = Check.NotNullOrWhiteSpace(sourceName, nameof(sourceName), EnkiProblemsConsts.MaxSourceNameLength, EnkiProblemsConsts.MinSourceNameLength);
        return this;
    }
    
    public Origin SetAuthorName(string authorName)
    {
        AuthorName = Check.NotNullOrWhiteSpace(authorName, nameof(authorName), EnkiProblemsConsts.MaxAuthorNameLength, EnkiProblemsConsts.MinAuthorNameLength);
        return this;
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ProblemId;
        yield return SourceName;
        yield return AuthorName;
    }
}