using System.Threading.Tasks;
using Fluid;

namespace Awesome.Net.Liquid
{
    public interface ILiquidTemplateEventHandler
    {
        Task RenderingAsync(TemplateContext context);
    }
}