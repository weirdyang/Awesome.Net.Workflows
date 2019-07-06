using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Liquid
{
    public class LiquidOptions
    {
        public IServiceCollection Services { get; }
        public Dictionary<string, Type> FilterRegistrations { get; } = new Dictionary<string, Type>();

        public LiquidOptions(IServiceCollection services)
        {
            Services = services;
        }
    }

    public static class LiquidOptionsExtensions
    {
        public static LiquidOptions RegisterFilter<T>(this LiquidOptions options) where T : class, ILiquidFilter
        {
            options.Services.AddTransient<T>();
            var filterName = LiquidFilterNameAttribute.GetName(typeof(T));
            options.FilterRegistrations.Add(filterName, typeof(T));
            return options;
        }
    }
}
