using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Test: FullAuditedEntity<int>
{
    public Guid ProblemId { get; set; }
    
    public int Score { get; set; }
    
    public bool IsUploaded { get; set; }

    private Test()
    {
    }
    
    internal Test(
        int id,
        Guid problemId,
        int score
    ): base(id)
    {
    }
}