using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    internal static class Extensions
    {
        public static bool TryGetValue<T>(this List<T> list, int index, out T value)
        {
            value = default;
            if (!IsIndexValid(list, index))
                return false;

            value = list[index];
            return true;
        }

        public static bool IsIndexValid<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}
