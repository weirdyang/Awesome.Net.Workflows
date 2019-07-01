using Awesome.Net.Workflows.Localization;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Awesome.Net.Workflows
{
    [DependsOn(typeof(AwesomeNetWorkflowsAbstractionsModule),
        typeof(AbpGuidsModule))]
    public class AwesomeNetWorkflowsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<VirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AwesomeNetWorkflowsAbstractionsModule>("Awesome.Net.Workflows",
                    "Awesome/Net/Workflows");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AwesomeNetWorkflowsResource>()
                    .AddVirtualJson("/Localization");
            });
        }
    }
}