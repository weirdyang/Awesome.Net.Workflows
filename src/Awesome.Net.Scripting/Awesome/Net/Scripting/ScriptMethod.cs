using System;

namespace Awesome.Net.Scripting
{
    public class ScriptMethod
    {
        public string Name { get; set; }
        public Func<IServiceProvider, Delegate> Method { get; set; }
    }
}
