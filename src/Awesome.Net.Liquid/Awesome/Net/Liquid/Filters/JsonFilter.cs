using System.Threading.Tasks;
using System.Web;
using Fluid;
using Fluid.Values;

namespace Awesome.Net.Liquid.Filters
{
    [LiquidFilterName("json")]
    public class JsonFilter : ILiquidFilter
    {
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var content = input.ToStringValue();

            if (string.IsNullOrWhiteSpace(content))
            {
                return new ValueTask<FluidValue>(input);
            }

            return new ValueTask<FluidValue>(
                new StringValue("\"" + HttpUtility.JavaScriptStringEncode(content) + "\""));
        }
    }
}
