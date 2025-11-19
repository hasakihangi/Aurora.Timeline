using System;
using System.Linq;


internal static class Utils
{
    public static bool ContainsAny<T>(T[] array, T[] items)
    {
        if (array == null || items == null) return false;
        if (array.Length == 0) return false;
        if (items.Length == 0) return false;

        for (int i = 0; i < items.Length; i++)
        {
            for (int j = 0; j < array.Length; j++)
            {
                if (Equals(array[j], items[i])) return true;
            }
        }

        return false;
    }

    public static bool Contains<T>(T[] array, T item)
    {
        if (array == null) return false;
        if (array.Length == 0) return false;
        return array.Contains(item);
    }
}
