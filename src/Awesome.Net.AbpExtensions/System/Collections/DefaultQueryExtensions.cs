namespace System.Collections
{
    public static class DefaultQueryExtensions
    {
        public static bool IsIn(this object source, IEnumerable values)
        {
            return false;
        }

        public static bool IsNotIn(this object source, IEnumerable values)
        {
            return false;
        }
    }
}