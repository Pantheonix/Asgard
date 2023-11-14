using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Problems;

public class Test : FullAuditedEntity<int>
{
    public Guid ProblemId { get; private set; }

    public int Score { get; private set; }

    public string InputDownloadUrl { get; private set; }

    public string OutputDownloadUrl { get; private set; }

    private Test() { }

    internal Test(
        int id,
        Guid problemId,
        int score,
        string inputDownloadUrl,
        string outputDownloadUrl
    )
        : base(id)
    {
        ProblemId = problemId;
        SetScore(score);
        SetDownloadUrls(inputDownloadUrl, outputDownloadUrl);
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

    internal Test SetDownloadUrls(string inputDownloadUrl, string outputDownloadUrl)
    {
        InputDownloadUrl = Check.NotNullOrEmpty(inputDownloadUrl, nameof(inputDownloadUrl));
        OutputDownloadUrl = Check.NotNullOrEmpty(outputDownloadUrl, nameof(outputDownloadUrl));

        if (!Uri.IsWellFormedUriString(InputDownloadUrl, UriKind.Absolute))
        {
            throw new ArgumentException(
                "Input download URL is not a valid absolute URL.",
                nameof(inputDownloadUrl)
            );
        }

        if (!Uri.IsWellFormedUriString(OutputDownloadUrl, UriKind.Absolute))
        {
            throw new ArgumentException(
                "Output download URL is not a valid absolute URL.",
                nameof(outputDownloadUrl)
            );
        }

        return this;
    }
}
