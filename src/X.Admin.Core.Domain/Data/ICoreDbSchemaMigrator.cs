using System.Threading.Tasks;

namespace X.Admin.Core.Data;

public interface ICoreDbSchemaMigrator
{
    Task MigrateAsync();
}
