using System;
using System.Collections.Generic;
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

    public EnkiProblemsTestDataSeedContributor(EnkiProblemsTestData testData, IRepository<Problem, Guid> problemRepository)
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
                "Problem Brief",
                "Problem Description",
                "Problem Source Name",
                "Problem Author Name",
                1,
                1,
                1,
                IoTypeEnum.Standard,
                DifficultyEnum.Easy,
                3,
                new List<ProgrammingLanguageEnum> { ProgrammingLanguageEnum.CSharp }
            )
        );
    }
}
