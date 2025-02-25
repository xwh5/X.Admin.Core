using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using X.Admin.BasicService;

namespace X.Admin.Core;

[DependsOn(
    typeof(CoreDomainModule),
    typeof(CoreApplicationContractsModule),
    typeof(BasicServiceApplicationModule)
    )]
public class CoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CoreApplicationModule>();
        });
    }
}
