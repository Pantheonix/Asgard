using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnkiProblems.Problems;

public class UpdateProblemDto
{
    [StringLength(
        EnkiProblemsConsts.MaxNameLength,
        MinimumLength = EnkiProblemsConsts.MinNameLength
    )]
    public string? Name { get; set; }

    [StringLength(
        EnkiProblemsConsts.MaxBriefLength,
        MinimumLength = EnkiProblemsConsts.MinBriefLength
    )]
    public string? Brief { get; set; }

    [StringLength(
        EnkiProblemsConsts.MaxDescriptionLength,
        MinimumLength = EnkiProblemsConsts.MinDescriptionLength
    )]
    public string? Description { get; set; }

    [StringLength(
        EnkiProblemsConsts.MaxSourceNameLength,
        MinimumLength = EnkiProblemsConsts.MinSourceNameLength
    )]
    public string? SourceName { get; set; }

    [StringLength(
        EnkiProblemsConsts.MaxAuthorNameLength,
        MinimumLength = EnkiProblemsConsts.MinAuthorNameLength
    )]
    public string? AuthorName { get; set; }

    public decimal? Time { get; set; }

    public decimal? TotalMemory { get; set; }

    public decimal? StackMemory { get; set; }

    public IoTypeEnum? IoType { get; set; }

    public DifficultyEnum? Difficulty { get; set; }

    [Range(EnkiProblemsConsts.MinNumberOfTests, EnkiProblemsConsts.MaxNumberOfTests)]
    public int? NumberOfTests { get; set; }

    public IEnumerable<ProgrammingLanguageEnum>? ProgrammingLanguages { get; set; }
}
