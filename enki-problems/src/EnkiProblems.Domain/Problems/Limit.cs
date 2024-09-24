using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace EnkiProblems.Problems;

public class Limit : ValueObject
{
    public Guid ProblemId { get; private set; }

    public decimal Time { get; private set; }

    public decimal TotalMemory { get; private set; }

    public decimal StackMemory { get; private set; }

    private Limit() { }

    internal Limit(Guid problemId, decimal time, decimal totalMemory, decimal stackMemory)
    {
        ProblemId = problemId;
        SetTime(time);
        SetMemory(totalMemory, stackMemory);
    }

    internal Limit SetTime(decimal time)
    {
        Time = Check.Range(
            time,
            nameof(time),
            EnkiProblemsConsts.MinTimeLimit,
            EnkiProblemsConsts.MaxTimeLimit
        );
        return this;
    }

    internal Limit SetMemory(decimal totalMemory, decimal stackMemory)
    {
        if (totalMemory < stackMemory)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TotalMemoryLessThanStackMemory
            );
        }

        TotalMemory = Check.Range(
            totalMemory,
            nameof(totalMemory),
            EnkiProblemsConsts.MinTotalMemoryLimit,
            EnkiProblemsConsts.MaxTotalMemoryLimit
        );

        StackMemory = Check.Range(
            stackMemory,
            nameof(stackMemory),
            EnkiProblemsConsts.MinStackMemoryLimit,
            EnkiProblemsConsts.MaxStackMemoryLimit
        );

        return this;
    }

    internal Limit SetTotalMemory(decimal totalMemory)
    {
        if (totalMemory < StackMemory)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TotalMemoryLessThanStackMemory
            );
        }

        TotalMemory = Check.Range(
            totalMemory,
            nameof(totalMemory),
            EnkiProblemsConsts.MinTotalMemoryLimit,
            EnkiProblemsConsts.MaxTotalMemoryLimit
        );

        return this;
    }

    internal Limit SetStackMemory(decimal stackMemory)
    {
        if (TotalMemory < stackMemory)
        {
            throw new BusinessException(
                EnkiProblemsDomainErrorCodes.TotalMemoryLessThanStackMemory
            );
        }

        StackMemory = Check.Range(
            stackMemory,
            nameof(stackMemory),
            EnkiProblemsConsts.MinStackMemoryLimit,
            EnkiProblemsConsts.MaxStackMemoryLimit
        );

        return this;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ProblemId;
        yield return Time;
        yield return TotalMemory;
        yield return StackMemory;
    }
}
