using X.Admin.Core.Samples;
using Xunit;

namespace X.Admin.Core.EntityFrameworkCore.Applications;

[Collection(CoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CoreEntityFrameworkCoreTestModule>
{

}
