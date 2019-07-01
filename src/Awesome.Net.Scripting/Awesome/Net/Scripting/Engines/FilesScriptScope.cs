using Microsoft.Extensions.FileProviders;

namespace Awesome.Net.Scripting.Engines
{
    public class FilesScriptScope : IScriptingScope
    {
        public FilesScriptScope(IFileProvider fileProvider, string basePath)
        {
            FileProvider = fileProvider;
            BasePath = basePath;
        }

        public IFileProvider FileProvider { get; }
        public string BasePath { get; }
    }
}
