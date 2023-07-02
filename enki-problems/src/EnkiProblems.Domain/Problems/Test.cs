using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Test : FullAuditedEntity<int>
{
    public Guid ProblemId { get; private set; }

    public int Score { get; private set; }

    public bool IsUploaded { get; private set; }

    private Test() { }

    internal Test(int id, Guid problemId, int score)
        : base(id) { }
}
