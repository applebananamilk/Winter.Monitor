using Volo.Abp.Autofac;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Winter.Monitor;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(WinterMonitorModule)
    )]
public class WinterMonitorTestModule : AbpModule
{

}
