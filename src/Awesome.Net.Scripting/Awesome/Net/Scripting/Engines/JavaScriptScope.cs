using System;
using Jint;

namespace Awesome.Net.Scripting.Engines
{
    public class JavaScriptScope : IScriptingScope
    {
        public JavaScriptScope(Engine engine, IServiceProvider serviceProvider)
        {
            Engine = engine;
            ServiceProvider = serviceProvider;
        }

        public Engine Engine { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
