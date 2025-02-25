using Volo.Abp.Modularity;
using X.Admin.BasicService;

namespace X.Admin.Core;

[DependsOn(
    typeof(CoreDomainSharedModule),
    typeof(BasicServiceApplicationContractsModule)
)]
public class CoreApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        CoreDtoExtensions.Configure();
    }
}
