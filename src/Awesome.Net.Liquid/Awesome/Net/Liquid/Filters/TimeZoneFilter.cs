using System;
using System.Globalization;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace Awesome.Net.Liquid.Filters
{
    [LiquidFilterName("local")]
    public class TimeZoneFilter : ILiquidFilter, IScopedDependency
    {
        private readonly IClock _clock;

        public TimeZoneFilter(IClock clock)
        {
            _clock = clock;
        }

        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            DateTime value;

            if(input.Type == FluidValues.String)
            {
                var stringValue = input.ToStringValue();

                if(stringValue == "now" || stringValue == "today")
                {
                    value = _clock.Now.ToLocalTime();
                }
                else
                {
                    if(!DateTime.TryParse(stringValue, context.CultureInfo, DateTimeStyles.AssumeUniversal, out value))
                    {
                        return new ValueTask<FluidValue>(NilValue.Instance);
                    }
                }
            }
            else
            {
                switch(input.ToObjectValue())
                {
                    case DateTime dateTime:
                        value = dateTime;
                        break;

                    case DateTimeOffset dateTimeOffset:
                        value = dateTimeOffset.DateTime;
                        break;

                    default:
                        return new ValueTask<FluidValue>(NilValue.Instance);
                }
            }

            return new ValueTask<FluidValue>(new ObjectValue(_clock.Normalize(value)));
        }
    }
}
