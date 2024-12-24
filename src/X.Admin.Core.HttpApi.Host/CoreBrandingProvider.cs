using Microsoft.Extensions.Localization;
using X.Admin.Core.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace X.Admin.Core;

[Dependency(ReplaceServices = true)]
public class CoreBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CoreResource> _localizer;

    public CoreBrandingProvider(IStringLocalizer<CoreResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
