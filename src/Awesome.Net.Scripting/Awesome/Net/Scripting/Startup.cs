using Awesome.Net.Scripting.Engines;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Scripting
{
    public static class Startup
    {
        public static IServiceCollection ConfigureScripting(this IServiceCollection services)
        {
            services.AddSingleton<IScriptingManager, DefaultScriptingManager>();

            services.AddGlobalMethodProvider<CommonMethodsProvider>();

            services.AddScriptingEngine<FilesScriptEngine>();
            services.AddScriptingEngine<JavaScriptEngine>();
            return services;
        }

        public static IServiceCollection AddScriptingEngine<T>(this IServiceCollection services)
            where T : class, IScriptingEngine
        {
            services.AddSingleton<IScriptingEngine, T>();
            return services;
        }

        public static IServiceCollection AddGlobalMethodProvider<T>(this IServiceCollection services)
            where T : class, IGlobalMethodProvider
        {
            services.AddSingleton<IGlobalMethodProvider, T>();
            return services;
        }
    }
}