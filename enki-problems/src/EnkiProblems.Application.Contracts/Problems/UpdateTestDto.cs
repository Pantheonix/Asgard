using System.ComponentModel.DataAnnotations;
using Volo.Abp.Content;

namespace EnkiProblems.Problems;

public class UpdateTestDto
{
    [Range(EnkiProblemsConsts.MinScore, EnkiProblemsConsts.MaxScore)]
    public int? Score { get; set; }

    public IRemoteStreamContent? ArchiveFile { get; set; }
}
