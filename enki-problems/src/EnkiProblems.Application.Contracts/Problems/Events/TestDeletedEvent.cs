using System;
using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems.Events;

public class TestDeletedEvent : EntityDto<int>
{
    public Guid ProblemId { get; set; }
}