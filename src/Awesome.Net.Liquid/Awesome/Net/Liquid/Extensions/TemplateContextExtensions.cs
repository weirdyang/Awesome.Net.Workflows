using System;
using Fluid;

namespace Awesome.Net.Liquid.Extensions
{
    public static class TemplateContextExtensions
    {
        public static void ContextualizeWithDefault(this TemplateContext context, IServiceProvider services)
        {
            if (!context.AmbientValues.ContainsKey("Services"))
            {
                context.AmbientValues.Add("Services", services);
            }
        }
    }
}
