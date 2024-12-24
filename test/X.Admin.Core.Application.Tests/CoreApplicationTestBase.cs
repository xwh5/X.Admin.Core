using Volo.Abp.Modularity;

namespace X.Admin.Core;

public abstract class CoreApplicationTestBase<TStartupModule> : CoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
