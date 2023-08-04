using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EnkiProblems.Problems;

public class UpdateTestDto
{
    [Range(EnkiProblemsConsts.MinScore, EnkiProblemsConsts.MaxScore)]
    public int? Score { get; set; }

    public IFormFile? ArchiveFile { get; set; }
}
