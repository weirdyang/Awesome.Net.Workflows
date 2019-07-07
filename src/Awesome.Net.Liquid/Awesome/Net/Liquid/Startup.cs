using System;
using System.Linq;
using Awesome.Net.Liquid.Filters;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Liquid
{
    public static class Startup
    {
        public static void ConfigureLiquid(this IServiceCollection services, Action<LiquidOptions> setupAction = null)
        {
            LiquidOptions options;
            if (services.Any(x => x.ServiceType == typeof(LiquidOptions)))
            {
                var sp = services.BuildServiceProvider();
                options = sp.GetService<LiquidOptions>();
                setupAction?.Invoke(options);
            }
            else
            {
                services.AddMemoryCache();

                options = new LiquidOptions(services);
                setupAction?.Invoke(options);
                options.AddCommonLiquidFilters();
                services.AddSingleton(options);

                services.AddTransient<ISlugService, SlugService>();
                services.AddTransient<ILiquidTemplateManager, LiquidTemplateManager>();

                ConfigureFluid();
            }
        }

        public static void AddLiquidFilter<T>(this IServiceCollection services) where T : class, ILiquidFilter
        {
            var sp = services.BuildServiceProvider();
            var options = sp.GetService<LiquidOptions>();
            options.AddFilter<T>();
        }

        private static void AddCommonLiquidFilters(this LiquidOptions options)
        {
            options.AddFilter<SlugifyFilter>()
                .AddFilter<JsonFilter>()
                .AddFilter<LiquidFilter>();
        }

        private static void ConfigureFluid()
        {
            // When accessing a property of a JObject instance
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((obj, name) => obj[name]);

            // Prevent JTokens from being converted to an ArrayValue as they implement IEnumerable
            FluidValue.TypeMappings.Add(typeof(JObject), o => new ObjectValue(o));
            FluidValue.TypeMappings.Add(typeof(JValue), o => FluidValue.Create(((JValue)o).Value));
            FluidValue.TypeMappings.Add(typeof(DateTime), o => new ObjectValue(o));
        }
    }
}
