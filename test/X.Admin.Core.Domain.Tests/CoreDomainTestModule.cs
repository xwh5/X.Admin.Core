using Volo.Abp.Modularity;

namespace X.Admin.Core;

[DependsOn(
    typeof(CoreDomainModule),
    typeof(CoreTestBaseModule)
)]
public class CoreDomainTestModule : AbpModule
{

}
