using EnkiProblems.Localization;
using Volo.Abp.Application.Services;

namespace EnkiProblems;

/* Inherit your application services from this class.
 */
public abstract class EnkiProblemsAppService : ApplicationService
{
    protected EnkiProblemsAppService()
    {
        LocalizationResource = typeof(EnkiProblemsResource);
    }
}
