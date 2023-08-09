using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EnkiProblems.Data;

/* This is used if database provider does't define
 * IEnkiProblemsDbSchemaMigrator implementation.
 */
public class NullEnkiProblemsDbSchemaMigrator : IEnkiProblemsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
