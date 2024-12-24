using Xunit;

namespace X.Admin.Core.EntityFrameworkCore;

[CollectionDefinition(CoreTestConsts.CollectionDefinitionName)]
public class CoreEntityFrameworkCoreCollection : ICollectionFixture<CoreEntityFrameworkCoreFixture>
{

}
