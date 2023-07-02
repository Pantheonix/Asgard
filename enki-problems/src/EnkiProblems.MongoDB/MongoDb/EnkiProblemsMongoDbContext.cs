using EnkiProblems.Problems;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace EnkiProblems.MongoDB;

[ConnectionStringName("Default")]
public class EnkiProblemsMongoDbContext : AbpMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    public IMongoCollection<Problem> Problems => Collection<Problem>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.Entity<Problem>(b =>
        {
            b.CollectionName = "Problems";
        });
    }
}
