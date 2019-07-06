using System;
using System.Collections.Generic;
using Awesome.Net.Scripting.Engines;
using Microsoft.Extensions.FileProviders;

namespace Awesome.Net.Scripting
{
    public interface IScriptingEngine
    {
        string Prefix { get; }
        object Evaluate(IScriptingScope scope, string script);

        IScriptingScope CreateScope(IEnumerable<ScriptMethod> methods, IServiceProvider serviceProvider,
            IFileProvider fileProvider, string basePath);
    }
}
