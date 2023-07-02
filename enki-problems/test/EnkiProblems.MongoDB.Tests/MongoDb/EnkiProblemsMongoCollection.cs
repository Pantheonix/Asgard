using Xunit;

namespace EnkiProblems.MongoDB;

[CollectionDefinition(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class EnkiProblemsMongoCollection : EnkiProblemsMongoDbCollectionFixtureBase { }
