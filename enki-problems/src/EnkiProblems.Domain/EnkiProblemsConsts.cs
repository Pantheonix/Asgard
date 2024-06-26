﻿namespace EnkiProblems;

public static class EnkiProblemsConsts
{
    public const string DbTablePrefix = "App";

    public const string DbSchema = null;

    public const string StateStoreName = "statestore";

    public const string PubSubName = "pubsub";

    public const string ProblemEvalMetadataUpsertedTopic = "problem-eval-metadata-upserted";

    public const string ProblemEvalMetadataDeletedTopic = "problem-eval-metadata-deleted";

    public const string TestUpsertedTopic = "test-upserted";

    public const string TestDeletedTopic = "test-deleted";

    public const string TestInputSuffix = "input";

    public const string TestOutputSuffix = "output";

    public const string ProposerRoleName = "Proposer";

    public const string AdminRoleName = "Admin";

    public const int MaxNameLength = 128;

    public const int MinNameLength = 2;

    public const int MaxBriefLength = 256;

    public const int MinBriefLength = 4;

    public const int MaxDescriptionLength = 4096;

    public const int MinDescriptionLength = 16;

    public const int MaxSourceNameLength = 128;

    public const int MinSourceNameLength = 2;

    public const int MaxAuthorNameLength = 128;

    public const int MinAuthorNameLength = 2;

    public const decimal MaxTimeLimit = 5m;

    public const decimal MinTimeLimit = 0.1m;

    public const decimal MaxTotalMemoryLimit = 512m;

    public const decimal MinTotalMemoryLimit = 0.1m;

    public const decimal MaxStackMemoryLimit = 64m;

    public const decimal MinStackMemoryLimit = 0.1m;

    public const int MaxNumberOfTests = 20;

    public const int MinScore = 0;

    public const int MaxScore = 100;

    public const int MaxTotalScore = 100;

    public const int TestChunkSize = 5000;
}
