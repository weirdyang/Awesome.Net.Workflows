using System;
using System.Collections.Generic;

namespace Awesome.Net.Liquid
{
    public class LiquidOptions
    {
        public Dictionary<string, Type> FilterRegistrations { get; }  = new Dictionary<string, Type>();
    }
}
