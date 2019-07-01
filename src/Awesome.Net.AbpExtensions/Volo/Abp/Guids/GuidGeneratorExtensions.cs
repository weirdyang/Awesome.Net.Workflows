namespace Volo.Abp.Guids
{
    public static class GuidGeneratorExtensions
    {
        public static string Create(this IGuidGenerator generator, string format = "N")
        {
            return generator.Create().ToString(format);
        }
    }
}