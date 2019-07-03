using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Awesome.Net.Liquid
{
    public class LiquidTemplateManager : ILiquidTemplateManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly LiquidOptions _liquidOptions;
        private readonly IServiceProvider _serviceProvider;

        public LiquidTemplateManager(
            IMemoryCache memoryCache,
            IOptions<LiquidOptions> options,
            IServiceProvider serviceProvider)
        {
            _memoryCache = memoryCache;
            _liquidOptions = options.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task RenderAsync(string source, TextWriter textWriter, TextEncoder encoder,
            TemplateContext context)
        {
            if(string.IsNullOrWhiteSpace(source))
            {
                return;
            }

            IEnumerable<string> errors;

            var result = _memoryCache.GetOrCreate(source, e =>
            {
                if(!LiquidViewTemplate.TryParse(source, out var parsed, out errors))
                {
                    // If the source string cannot be parsed, create a template that contains the parser errors
                    LiquidViewTemplate.TryParse(string.Join(Environment.NewLine, errors), out parsed, out errors);
                }

                // Define a default sliding expiration to prevent the 
                // cache from being filled and still apply some micro-caching
                // in case the template is use commonly
                e.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                return parsed;
            });

            await context.ContextualizeAsync(_serviceProvider);
            await result.RenderAsync(_liquidOptions, _serviceProvider, textWriter, encoder, context);
        }

        public bool Validate(string template, out IEnumerable<string> errors)
        {
            return LiquidViewTemplate.TryParse(template, out _, out errors);
        }
    }
}