using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EnkiProblems.Problems;

public class CreateTestDto
{
    [Required]
    [Range(EnkiProblemsConsts.MinScore, EnkiProblemsConsts.MaxScore)]
    public int Score { get; set; }

    [Required]
    public IFormFile ArchiveFile { get; set; }
}
