using System;
using System.Linq;
using Awesome.Net.Scripting.Engines;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Scripting
{
    public static class Startup
    {
        public static IServiceCollection AddScripting(this IServiceCollection services,
            Action<ScriptingOptions> setupAction = null)
        {
            if (services.Any(x => x.ServiceType == typeof(ScriptingOptions)))
            {
                throw new InvalidOperationException("Scripting services already registered");
            }

            var options = new ScriptingOptions(services);
            setupAction?.Invoke(options);
            services.AddSingleton(options);

            services.AddSingleton<IScriptingManager, DefaultScriptingManager>();

            services.AddSingleton<IScriptingEngine, FilesScriptEngine>();
            services.AddSingleton<IScriptingEngine, JavaScriptEngine>();

            services.RegisterGlobalScriptMethodProvider<CommonScriptMethodProvider>();

            return services;
        }

        public static IServiceCollection RegisterGlobalScriptMethodProvider<T>(this IServiceCollection services)
            where T : class, IScriptMethodProvider
        {
            services.AddSingleton<IScriptMethodProvider, T>();
            return services;
        }
    }
}
