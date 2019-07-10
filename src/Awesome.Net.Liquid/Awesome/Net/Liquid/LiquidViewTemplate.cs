using System;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Awesome.Net.Liquid
{
    public class LiquidViewTemplate : BaseFluidTemplate<LiquidViewTemplate>
    {
        public static LiquidViewTemplate Parse(string path, IFileProvider fileProvider, IMemoryCache cache,
            bool isDevelopment)
        {
            return cache.GetOrCreate(path, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(1));
                var fileInfo = fileProvider.GetFileInfo(path);

                if (isDevelopment)
                {
                    entry.ExpirationTokens.Add(fileProvider.Watch(path));
                }

                using (var stream = fileInfo.CreateReadStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        if (TryParse(sr.ReadToEnd(), out var template, out var errors))
                        {
                            return template;
                        }
                        else
                        {
                            throw new Exception(
                                $"Failed to parse liquid file {path}: {string.Join(Environment.NewLine, errors)}");
                        }
                    }
                }
            });
        }
    }

    public static class LiquidViewTemplateExtensions
    {
        public static Task RenderAsync(this LiquidViewTemplate template, LiquidOptions options,
            IServiceProvider services, TextWriter writer, TextEncoder encoder, TemplateContext templateContext)
        {
            foreach (var registration in options.FilterRegistrations)
            {
                templateContext.Filters.AddAsyncFilter(registration.Key, (input, arguments, ctx) =>
                {
                    var filter = services.GetRequiredService(registration.Value) as ILiquidFilter;
                    return filter.ProcessAsync(input, arguments, ctx);
                });
            }

            // Otherwise, we don't need the view engine for rendering.
            return template.RenderAsync(writer, encoder, templateContext);
        }
    }

    public static class TemplateContextExtensions
    {
        public static async Task ContextualizeAsync(this TemplateContext context, IServiceProvider services)
        {
            if (!context.AmbientValues.ContainsKey("Services"))
            {
                context.AmbientValues.Add("Services", services);
            }

            context.CultureInfo = CultureInfo.CurrentUICulture;

            foreach (var handler in services.GetServices<ILiquidTemplateEventHandler>())
            {
                await handler.RenderingAsync(context);
            }
        }
    }
}
