using Asgard.Hermes;

namespace EnkiProblems.Problems.Tests;

public class DownloadTestStreamDto
{
    public string ProblemId { get; set; }
    public string TestId { get; set; }
    public StatusCode StatusCode { get; set; }
    public byte[] TestArchiveBytes { get; set; }
}
