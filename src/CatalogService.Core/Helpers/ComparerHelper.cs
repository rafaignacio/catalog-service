namespace CatalogService.Core.Helpers;

public static class ComparerHelper
{
    public static T CompareField<T>(T x, T y) where T : IComparable<T>
    {
        if (x == null || x.CompareTo(default) == 0) return y;
        if (x.CompareTo(y) != 0) return x;

        return y;
    }
}
