using System.ComponentModel.DataAnnotations;

namespace EnkiProblems.Problems;

public class CreateProblemDto
{
    [Required]
    [StringLength(
        EnkiProblemsConsts.MaxNameLength,
        MinimumLength = EnkiProblemsConsts.MinNameLength
    )]
    public string Name { get; set; }

    [Required]
    [StringLength(
        EnkiProblemsConsts.MaxBriefLength,
        MinimumLength = EnkiProblemsConsts.MinBriefLength
    )]
    public string Brief { get; set; }

    [Required]
    [StringLength(
        EnkiProblemsConsts.MaxDescriptionLength,
        MinimumLength = EnkiProblemsConsts.MinDescriptionLength
    )]
    public string Description { get; set; }

    [Required]
    [StringLength(
        EnkiProblemsConsts.MaxSourceNameLength,
        MinimumLength = EnkiProblemsConsts.MinSourceNameLength
    )]
    public string SourceName { get; set; }

    [Required]
    [StringLength(
        EnkiProblemsConsts.MaxAuthorNameLength,
        MinimumLength = EnkiProblemsConsts.MinAuthorNameLength
    )]
    public string AuthorName { get; set; }

    [Required]
    public decimal Time { get; set; }

    [Required]
    public decimal TotalMemory { get; set; }

    [Required]
    public decimal StackMemory { get; set; }

    [Required]
    public IoTypeEnum IoType { get; set; }

    [Required]
    public DifficultyEnum Difficulty { get; set; }
}
