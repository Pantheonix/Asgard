using EnkiProblems.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EnkiProblems.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class EnkiProblemsController : AbpControllerBase
{
    protected EnkiProblemsController()
    {
        LocalizationResource = typeof(EnkiProblemsResource);
    }
}
