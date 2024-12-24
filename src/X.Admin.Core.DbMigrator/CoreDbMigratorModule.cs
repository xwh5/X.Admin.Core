using X.Admin.Core.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace X.Admin.Core.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CoreEntityFrameworkCoreModule),
    typeof(CoreApplicationContractsModule)
)]
public class CoreDbMigratorModule : AbpModule
{
}
