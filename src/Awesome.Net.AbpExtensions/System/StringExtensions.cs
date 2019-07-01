namespace System
{
    public static class StringExtensions
    {
        public static string ToSentenceCase(this string str, string separator, bool useCurrentCulture = false)
        {
            return str.ToSentenceCase(useCurrentCulture).Replace(" ", separator);
        }
    }
}
