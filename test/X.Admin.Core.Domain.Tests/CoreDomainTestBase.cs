using Volo.Abp.Modularity;

namespace X.Admin.Core;

/* Inherit from this class for your domain layer tests. */
public abstract class CoreDomainTestBase<TStartupModule> : CoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
