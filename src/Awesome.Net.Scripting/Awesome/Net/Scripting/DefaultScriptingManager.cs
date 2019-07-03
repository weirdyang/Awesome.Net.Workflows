using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Awesome.Net.Scripting
{
    public class DefaultScriptingManager : IScriptingManager
    {
        private readonly IEnumerable<IScriptingEngine> _engines;
        private readonly IServiceProvider _serviceProvider;

        public DefaultScriptingManager(
            IEnumerable<IScriptingEngine> engines,
            IEnumerable<IGlobalMethodProvider> globalMethodProviders,
            IServiceProvider serviceProvider)
        {
            _engines = engines;
            _serviceProvider = serviceProvider;
            GlobalMethodProviders = new List<IGlobalMethodProvider>(globalMethodProviders);
        }

        public IList<IGlobalMethodProvider> GlobalMethodProviders { get; }

        public object Evaluate(string directive,
            IFileProvider fileProvider,
            string basePath,
            IEnumerable<IGlobalMethodProvider> scopedMethodProviders)
        {
            if(directive == null) throw new ArgumentNullException(nameof(directive));

            var directiveIndex = directive.IndexOf(":", StringComparison.Ordinal);

            if(directiveIndex == -1 || directiveIndex >= directive.Length - 2)
            {
                return directive;
            }

            var prefix = directive.Substring(0, directiveIndex);
            var script = directive.Substring(directiveIndex + 1);

            var engine = GetScriptingEngine(prefix);
            if(engine == null)
            {
                return directive;
            }

            var methodProviders = scopedMethodProviders != null ? GlobalMethodProviders.Concat(scopedMethodProviders) : GlobalMethodProviders;

            using(var scope = _serviceProvider.CreateScope())
            {
                var scriptingScope = engine.CreateScope(methodProviders.SelectMany(x => x.GetMethods()), scope.ServiceProvider, fileProvider, basePath);

                return engine.Evaluate(scriptingScope, script);
            }
        }

        public IScriptingEngine GetScriptingEngine(string prefix)
        {
            return _engines.FirstOrDefault(x => x.Prefix == prefix);
        }
    }
}
