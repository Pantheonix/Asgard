using System;
using Volo.Abp.Application.Dtos;

namespace EnkiProblems.Problems.Events;

public class TestUpsertedEvent : EntityDto<int>
{
    public Guid ProblemId { get; set; }

    public string InputDownloadUrl { get; set; }

    public string OutputDownloadUrl { get; set; }
}