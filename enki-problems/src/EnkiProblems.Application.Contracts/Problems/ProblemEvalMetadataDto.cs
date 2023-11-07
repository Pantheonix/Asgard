using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems;

public class ProblemEvalMetadataDto : EntityDto<Guid>
{
    public string Name { get; set; }
    
    public Guid ProposerId { get; set; }
    
    public bool IsPublished { get; set; }

    public decimal Time { get; set; }

    public decimal TotalMemory { get; set; }

    public decimal StackMemory { get; set; }

    public IoTypeEnum IoType { get; set; }

    public IEnumerable<TestDto> Tests { get; set; }
}
