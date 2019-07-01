using System;
using System.Collections.Generic;
using Awesome.Net.Liquid;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Localization;
using Awesome.Net.Workflows.Services;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;
using Volo.Abp.VirtualFileSystem;

namespace Awesome.Net.Workflows
{
    [DependsOn(typeof(AwesomeNetLiquidModule),
        typeof(AbpLocalizationModule),
        typeof(AwesomeNetScriptingModule))]
    public class AwesomeNetWorkflowsAbstractionsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            RegisterActivities(context.Services);
        }

        private static void RegisterActivities(IServiceCollection services)
        {
            var activityTypes = new List<Type>();

            services.OnRegistred(context =>
            {
                if(ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(IActivity)))
                {
                    activityTypes.Add(context.ImplementationType);
                }
            });

            services.Configure<WorkflowOptions>(options =>
            {
                foreach(var type in activityTypes)
                {
                    options.RegisterActivity(type);
                }
            });
        }

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
                    .Add<AwesomeNetWorkflowsResource>("en")
                    .AddVirtualJson("/Localization");
            });
        }
    }
}