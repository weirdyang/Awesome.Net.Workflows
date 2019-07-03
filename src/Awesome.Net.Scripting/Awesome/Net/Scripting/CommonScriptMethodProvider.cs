using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Awesome.Net.Scripting
{
    public class CommonScriptMethodProvider : IScriptMethodProvider
    {
        private static ScriptMethod Base64 = new ScriptMethod
        {
            Name = "base64",
            Method = serviceProvider => (Func<string, object>) (encoded =>
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            })
        };

        private static ScriptMethod Html = new ScriptMethod
        {
            Name = "html",
            Method = serviceProvider => (Func<string, object>) (encoded => { return WebUtility.HtmlDecode(encoded); })
        };

        /// <summary>
        /// Converts a Base64 encoded gzip stream to an uncompressed Base64 string.
        /// See http://www.txtwizard.net/compression
        /// </summary>
        private static ScriptMethod GZip = new ScriptMethod
        {
            Name = "gzip",
            Method = serviceProvider => (Func<string, object>) (encoded =>
            {
                var bytes = Convert.FromBase64String(encoded);
                using (var gzip = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
                {
                    var decompressed = new MemoryStream();
                    var buffer = new byte[1024];
                    int nRead;
                    while ((nRead = gzip.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        decompressed.Write(buffer, 0, nRead);
                    }

                    return Convert.ToBase64String(decompressed.ToArray());
                }
            })
        };

        private static ScriptMethod Uuid = new ScriptMethod
        {
            Name = "uuid", Method = serviceProvider => (Func<object>) (() => { return Guid.NewGuid(); })
        };

        public IEnumerable<ScriptMethod> GetMethods()
        {
            return new[] {Base64, Html, GZip, Uuid};
        }
    }
}
