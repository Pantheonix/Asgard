using System;
using System.Threading.Tasks;
using EnkiProblems.Problems;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace EnkiProblems;

public class EnkiProblemsTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly EnkiProblemsTestData _testData;
    private readonly IRepository<Problem, Guid> _problemRepository;

    public EnkiProblemsTestDataSeedContributor(
        EnkiProblemsTestData testData,
        IRepository<Problem, Guid> problemRepository
    )
    {
        _testData = testData;
        _problemRepository = problemRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        await _problemRepository.InsertAsync(
            new Problem(
                _testData.ProblemId1,
                _testData.ProblemName1,
                _testData.ProblemBrief1,
                _testData.ProblemDescription1,
                _testData.ProblemSourceName1,
                _testData.ProblemAuthorName1,
                _testData.ProblemProposerId1,
                _testData.ProblemTimeLimit1,
                _testData.ProblemTotalMemoryLimit1,
                _testData.ProblemStackMemoryLimit1,
                _testData.ProblemIoType1,
                _testData.ProblemDifficulty1,
                _testData.ProblemNumberOfTests1,
                _testData.ProblemProgrammingLanguages1
            )
        );
    }
}
