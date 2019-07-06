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
        public static void AddLiquid(this IServiceCollection services, Action<LiquidOptions> setupAction = null)
        {
            if (services.Any(x => x.ServiceType == typeof(LiquidOptions)))
            {
                throw new InvalidOperationException("Liquid services already registered");
            }

            var options = new LiquidOptions(services);
            setupAction?.Invoke(options);
            options.AddCommonLiquidFilters();
            services.AddSingleton(options);

            services.AddTransient<ISlugService, SlugService>();
            services.AddTransient<ILiquidTemplateManager, LiquidTemplateManager>();

            ConfigureFluid();
        }

        private static void AddCommonLiquidFilters(this LiquidOptions options)
        {
            options.RegisterFilter<SlugifyFilter>()
                .RegisterFilter<JsonFilter>()
                .RegisterFilter<LiquidFilter>();
        }

        private static void ConfigureFluid()
        {
            // When accessing a property of a JObject instance
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((obj, name) => obj[name]);

            // Prevent JTokens from being converted to an ArrayValue as they implement IEnumerable
            FluidValue.TypeMappings.Add(typeof(JObject), o => new ObjectValue(o));
            FluidValue.TypeMappings.Add(typeof(JValue), o => FluidValue.Create(((JValue) o).Value));
            FluidValue.TypeMappings.Add(typeof(DateTime), o => new ObjectValue(o));
        }
    }
}
