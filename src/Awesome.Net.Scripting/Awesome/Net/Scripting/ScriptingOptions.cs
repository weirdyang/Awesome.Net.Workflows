using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Scripting
{
    public class ScriptingOptions
    {
        public IServiceCollection Services { get; }

        public ScriptingOptions(IServiceCollection services)
        {
            Services = services;
        }
    }
}
