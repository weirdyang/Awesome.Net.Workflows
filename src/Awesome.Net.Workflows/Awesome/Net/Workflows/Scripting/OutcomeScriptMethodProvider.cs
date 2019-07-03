using System;
using System.Collections.Generic;
using Awesome.Net.Scripting;

namespace Awesome.Net.Workflows.Scripting
{
    public class OutcomeScriptMethodProvider : IScriptMethodProvider
    {
        private readonly ScriptMethod _setOutcomeMethod;

        public OutcomeScriptMethodProvider(IList<string> outcomes)
        {
            _setOutcomeMethod = new ScriptMethod
            {
                Name = "setOutcome", Method = serviceProvider => (Action<string>) (name => outcomes.Add(name))
            };
        }

        public IEnumerable<ScriptMethod> GetMethods()
        {
            return new[] {_setOutcomeMethod};
        }
    }
}
