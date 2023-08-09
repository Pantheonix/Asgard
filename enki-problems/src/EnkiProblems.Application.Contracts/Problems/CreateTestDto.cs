using System.ComponentModel.DataAnnotations;
using Volo.Abp.Content;

namespace EnkiProblems.Problems;

public class CreateTestDto
{
    [Required]
    [Range(EnkiProblemsConsts.MinScore, EnkiProblemsConsts.MaxScore)]
    public int Score { get; set; }

    [Required]
    public IRemoteStreamContent ArchiveFile { get; set; }
}
