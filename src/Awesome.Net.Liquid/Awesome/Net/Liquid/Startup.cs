using Awesome.Net.Liquid.Filters;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Liquid
{
    public static class Startup
    {
        public static void ConfigureLiquid(this IServiceCollection services)
        {
            services.AddTransient<ISlugService, SlugService>();
            services.AddTransient<ILiquidTemplateManager, LiquidTemplateManager>();

            services.AddLiquidFilter<SlugifyFilter>(LiquidFilterNameAttribute.GetName(typeof(SlugifyFilter)));
            services.AddLiquidFilter<JsonFilter>(LiquidFilterNameAttribute.GetName(typeof(JsonFilter)));
            services.AddLiquidFilter<LiquidFilter>(LiquidFilterNameAttribute.GetName(typeof(LiquidFilter)));

            ConfigureFluid();
        }

        public static IServiceCollection AddLiquidFilter<T>(this IServiceCollection services, string name) where T : class, ILiquidFilter
        {
            services.AddScoped<T>();
            services.Configure<LiquidOptions>(options => options.FilterRegistrations.Add(name, typeof(T)));
            return services;
        }

        private static void ConfigureFluid()
        {
            // When accessing a property of a JObject instance
            TemplateContext.GlobalMemberAccessStrategy.Register<JObject, object>((obj, name) => obj[name]);

            // Prevent JTokens from being converted to an ArrayValue as they implement IEnumerable
            FluidValue.TypeMappings.Add(typeof(JObject), o => new ObjectValue(o));
            FluidValue.TypeMappings.Add(typeof(JValue), o => FluidValue.Create(((JValue)o).Value));
            FluidValue.TypeMappings.Add(typeof(System.DateTime), o => new ObjectValue(o));
        }
    }
}