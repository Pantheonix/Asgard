using EnkiProblems.MongoDB;
using Xunit;

namespace EnkiProblems;

[CollectionDefinition(EnkiProblemsTestConsts.CollectionDefinitionName)]
public class EnkiProblemsDomainCollection : EnkiProblemsMongoDbCollectionFixtureBase
{

}
