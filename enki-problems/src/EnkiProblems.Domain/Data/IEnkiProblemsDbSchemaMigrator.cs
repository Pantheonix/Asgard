using System.Threading.Tasks;

namespace EnkiProblems.Data;

public interface IEnkiProblemsDbSchemaMigrator
{
    Task MigrateAsync();
}
