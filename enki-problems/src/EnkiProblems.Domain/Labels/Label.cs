using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace EnkiProblems.Labels;

public class Label : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
}
