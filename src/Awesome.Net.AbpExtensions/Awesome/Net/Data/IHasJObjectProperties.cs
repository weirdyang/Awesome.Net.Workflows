using Newtonsoft.Json.Linq;

namespace Awesome.Net.Data
{
    public interface IHasJObjectProperties
    {
        JObject Properties { get; set; }
    }
}