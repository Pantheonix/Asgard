using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems;

public class ProblemWithTestsDto : EntityDto<Guid>
{
    public string Name { get; set; }

    public string Brief { get; set; }

    public string Description { get; set; }

    public bool IsPublished { get; set; }

    public string SourceName { get; set; }

    public string AuthorName { get; set; }

    public decimal Time { get; set; }

    public decimal TotalMemory { get; set; }

    public decimal StackMemory { get; set; }

    public Guid ProposerId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? PublishingDate { get; set; }

    public IoTypeEnum IoType { get; set; }

    public DifficultyEnum Difficulty { get; set; }

    public int NumberOfTests { get; set; }

    public IEnumerable<ProgrammingLanguageEnum> ProgrammingLanguages { get; set; }

    public IEnumerable<TestDto> Tests { get; set; }
}
