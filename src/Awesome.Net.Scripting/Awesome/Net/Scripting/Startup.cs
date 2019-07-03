using Awesome.Net.Scripting.Engines;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Scripting
{
    public static class Startup
    {
        public static IServiceCollection ConfigureScripting(this IServiceCollection services)
        {
            services.AddSingleton<IScriptingManager, DefaultScriptingManager>();

            services.AddSingleton<IScriptingEngine, FilesScriptEngine>();
            services.AddSingleton<IScriptingEngine, JavaScriptEngine>();

            services.AddGlobalScriptMethodProvider<CommonScriptMethodProvider>();

            return services;
        }

        public static IServiceCollection AddGlobalScriptMethodProvider<T>(this IServiceCollection services)
            where T : class, IScriptMethodProvider
        {
            services.AddSingleton<IScriptMethodProvider, T>();
            return services;
        }
    }
}
