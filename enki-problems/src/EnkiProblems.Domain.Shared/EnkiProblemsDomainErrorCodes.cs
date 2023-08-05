namespace EnkiProblems;

public static class EnkiProblemsDomainErrorCodes
{
    public const string ProblemNameAlreadyExists = "EnkiProblems:ProblemNameAlreadyExists";

    public const string TotalMemoryLessThanStackMemory =
        "EnkiProblems:TotalMemoryLessThanStackMemory";

    public const string NotAllowedToCreateProblem = "EnkiProblems:NotAllowedToCreateProblem";

    public const string NotAllowedToViewUnpublishedProblems =
        "EnkiProblems:NotAllowedToViewUnpublishedProblems";

    public const string NotAllowedToEditProblem = "EnkiProblems:NotAllowedToEditProblem";

    public const string NotAllowedToEditPublishedProblem =
        "EnkiProblems:NotAllowedToEditPublishedProblem";

    public const string UnpublishedProblemNotBelongingToCurrentUser =
        "EnkiProblems:UnpublishedProblemNotBelongingToCurrentUser";

    public const string ProblemNotFound = "EnkiProblems:ProblemNotFound";

    public const string TestNotFound = "EnkiProblems:TestNotFound";

    public const string NumberOfTestsExceedsLimit = "EnkiProblems:NumberOfTestsExceedsLimit";

    public const string TestUploadFailed = "EnkiProblems:TestUploadFailed";

    public const string TestDeleteFailed = "EnkiProblems:TestDeleteFailed";

    public const string DownloadTestArchiveError = "EnkiProblems:DownloadTestArchiveError";

    public const string TotalScoreExceeded = "EnkiProblems:TotalScoreExceeded";

    public const string NumberOfTestsExceeded = "EnkiProblems:NumberOfTestsExceeded";

    public const string TestDownloadUrlRetrievalFailed =
        "EnkiProblems:TestDownloadUrlRetrievalFailed";
    
    public const string ProblemCannotBePublished = "EnkiProblems:ProblemCannotBePublished";
}
