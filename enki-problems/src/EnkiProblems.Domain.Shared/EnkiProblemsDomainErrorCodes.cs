﻿namespace EnkiProblems;

public static class EnkiProblemsDomainErrorCodes
{
    public const string ProblemAlreadyExists = "EnkiProblems:ProblemAlreadyExists";

    public const string TotalMemoryLessThanStackMemory =
        "EnkiProblems:TotalMemoryLessThanStackMemory";

    public const string NotAllowedToCreateProblem = "EnkiProblems:NotAllowedToCreateProblem";

    public const string NotAllowedToViewUnpublishedProblems =
        "EnkiProblems:NotAllowedToViewUnpublishedProblems";

    public const string UnpublishedProblemNotBelongingToCurrentUser =
        "EnkiProblems:UnpublishedProblemNotBelongingToCurrentUser";

    public const string ProblemNotFound = "EnkiProblems:ProblemNotFound";
}
