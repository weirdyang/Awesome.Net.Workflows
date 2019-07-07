using System;
using System.Linq;
using Awesome.Net.Scripting.Engines;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Scripting
{
    public static class Startup
    {
        public static IServiceCollection ConfigureScripting(this IServiceCollection services,
            Action<ScriptingOptions> setupAction = null)
        {
            ScriptingOptions options;
            if (services.Any(x => x.ServiceType == typeof(ScriptingOptions)))
            {
                var sp = services.BuildServiceProvider();
                options = sp.GetService<ScriptingOptions>();
                setupAction?.Invoke(options);
            }
            else
            {
                services.AddMemoryCache();

                options = new ScriptingOptions(services);
                setupAction?.Invoke(options);
                services.AddSingleton(options);

                services.AddSingleton<IScriptingManager, DefaultScriptingManager>();

                services.AddSingleton<IScriptingEngine, FilesScriptEngine>();
                services.AddSingleton<IScriptingEngine, JavaScriptEngine>();

                services.AddGlobalScriptMethodProvider<CommonScriptMethodProvider>();
            }

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
