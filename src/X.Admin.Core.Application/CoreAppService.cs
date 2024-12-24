using X.Admin.Core.Localization;
using Volo.Abp.Application.Services;

namespace X.Admin.Core;

/* Inherit your application services from this class.
 */
public abstract class CoreAppService : ApplicationService
{
    protected CoreAppService()
    {
        LocalizationResource = typeof(CoreResource);
    }
}
