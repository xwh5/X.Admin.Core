using X.Admin.Core.Samples;
using Xunit;

namespace X.Admin.Core.EntityFrameworkCore.Domains;

[Collection(CoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CoreEntityFrameworkCoreTestModule>
{

}
