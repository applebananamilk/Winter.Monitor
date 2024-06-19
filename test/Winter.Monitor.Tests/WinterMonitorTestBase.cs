using Volo.Abp;
using Volo.Abp.Testing;

namespace Winter.Monitor;

public abstract class WinterMonitorTestBase : AbpIntegratedTest<WinterMonitorTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
