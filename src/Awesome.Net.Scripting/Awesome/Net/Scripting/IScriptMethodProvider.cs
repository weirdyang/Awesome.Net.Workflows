using System.Collections.Generic;

namespace Awesome.Net.Scripting
{
    /// <summary>
    /// An implementation of <see cref="IScriptMethodProvider"/> provides custom methods for
    /// an <see cref="IScriptingManager"/> intance.
    /// </summary>
    public interface IScriptMethodProvider
    {
        /// <summary>
        /// Gets the list of methods to provide to the <see cref="IScriptingManager"/>.
        /// </summary>
        /// <returns>A list of <see cref="ScriptMethod"/> instances.</returns>
        IEnumerable<ScriptMethod> GetMethods();
    }
}
