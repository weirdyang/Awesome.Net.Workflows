using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Liquid.Filters
{
    [LiquidFilterName("slugify")]
    public class SlugifyFilter : ILiquidFilter, IScopedDependency
    {
        private readonly ISlugService _slugService;

        public SlugifyFilter(ISlugService slugService)
        {
            _slugService = slugService;
        }

        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var text = input.ToStringValue();

            return new ValueTask<FluidValue>(new StringValue(_slugService.Slugify(text)));
        }
    }
}
