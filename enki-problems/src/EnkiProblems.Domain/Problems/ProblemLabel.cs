using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace EnkiProblems.Problems;

public class ProblemLabel: ValueObject
{
    public Guid ProblemId { get; private set; }
    
    public Guid LabelId { get; private set; }
    
    private ProblemLabel()
    {
    }
    
    internal ProblemLabel(
        Guid problemId,
        Guid labelId
    )
    {
        ProblemId = problemId;
        LabelId = labelId;
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ProblemId;
        yield return LabelId;
    }
}