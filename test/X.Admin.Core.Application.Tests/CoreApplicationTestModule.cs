using Volo.Abp.Modularity;

namespace X.Admin.Core;

[DependsOn(
    typeof(CoreApplicationModule),
    typeof(CoreDomainTestModule)
)]
public class CoreApplicationTestModule : AbpModule
{

}
