using Fluid;

namespace Awesome.Net.Liquid.Events
{
    public class LiquidTemplateRenderingEventData
    {
        public TemplateContext TemplateContext { get; }

        public LiquidTemplateRenderingEventData(TemplateContext templateContext)
        {
            TemplateContext = templateContext;
        }
    }
}