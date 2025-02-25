using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using X.Admin.BasicService.EntityFrameworkCore;

namespace X.Admin.Core.EntityFrameworkCore;

[DependsOn(
    typeof(CoreDomainModule),
    typeof(BasicServiceEntityFrameworkCoreModule)
    )]
public class CoreEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {

        CoreEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CoreDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
                /* The main point to change your DBMS.
                 * See also CoreDbContextFactory for EF Core tooling. */
            options.UseMySQL();
        });
        
    }
}
