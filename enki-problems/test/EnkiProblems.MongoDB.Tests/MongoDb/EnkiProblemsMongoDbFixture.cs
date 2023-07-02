using System;
using Mongo2Go;

namespace EnkiProblems.MongoDB;

public class EnkiProblemsMongoDbFixture : IDisposable
{
    private static readonly MongoDbRunner MongoDbRunner;
    public static readonly string ConnectionString;

    static EnkiProblemsMongoDbFixture()
    {
        MongoDbRunner = MongoDbRunner.Start(
            singleNodeReplSet: true,
            singleNodeReplSetWaitTimeout: 20
        );
        ConnectionString = MongoDbRunner.ConnectionString;
    }

    public void Dispose()
    {
        MongoDbRunner?.Dispose();
    }
}
