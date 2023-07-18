using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems;

public class ProblemListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Name { get; set; }

    public Guid? ProposerId { get; set; }

    public IoTypeEnum? IoType { get; set; }

    public DifficultyEnum? Difficulty { get; set; }

    public IEnumerable<ProgrammingLanguageEnum>? ProgrammingLanguages { get; set; }
}
