using Localization.Resources.AbpUi;
using X.Admin.Core.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using X.Admin.BasicService;

namespace X.Admin.Core;

[DependsOn(
    typeof(CoreApplicationContractsModule),
    typeof(BasicServiceHttpApiModule)
    )]
public class CoreHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CoreResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
