using System;
using System.Collections.Generic;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;
using Volo.Abp.Timing;

namespace Awesome.Net.Liquid
{
    [DependsOn(typeof(AbpTimingModule),
        typeof(AbpEventBusModule))]
    public class AwesomeNetLiquidModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            RegisterLiquidFilters(context.Services);
            ConfigureFluid();
        }

        private void ConfigureFluid()
        {
            // When accessing a property of a JObject instance
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((obj, name) => obj[name]);

            // Prevent JTokens from being converted to an ArrayValue as they implement IEnumerable
            FluidValue.TypeMappings.Add(typeof(JObject), o => new ObjectValue(o));
            FluidValue.TypeMappings.Add(typeof(JValue), o => FluidValue.Create(((JValue)o).Value));
            FluidValue.TypeMappings.Add(typeof(DateTime), o => new ObjectValue(o));
        }

        private static void RegisterLiquidFilters(IServiceCollection services)
        {
            var liquidFilterTypes = new List<Type>();

            services.OnRegistred(context =>
            {
                if(ReflectionHelper.IsAssignableToGenericType(context.ImplementationType, typeof(ILiquidFilter)))
                {
                    liquidFilterTypes.Add(context.ImplementationType);
                }
            });

            services.Configure<LiquidOptions>(options =>
            {
                foreach(var filterType in liquidFilterTypes)
                {
                    var name = LiquidFilterNameAttribute.GetFilterName(filterType);
                    options.FilterRegistrations.Add(name, filterType);
                }
            });
        }
    }
}