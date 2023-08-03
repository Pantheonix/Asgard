using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Test : FullAuditedEntity<int>
{
    public Guid ProblemId { get; private set; }

    public int Score { get; private set; }

    private Test() { }

    internal Test(int id, Guid problemId, int score)
        : base(id)
    {
        ProblemId = problemId;
        SetScore(score);
    }

    internal Test SetScore(int score)
    {
        Score = Check.Range(
            score,
            nameof(score),
            EnkiProblemsConsts.MinScore,
            EnkiProblemsConsts.MaxScore
        );
        return this;
    }
}
