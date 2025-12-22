using System;
using System.Collections.Generic;

namespace Aurora.Timeline
{
    internal static class EnumerableHelpers
    {
        internal static void Copy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)
        {
            if (source is ICollection<T> objs)
                objs.CopyTo(array, arrayIndex);
            else
                EnumerableHelpers.IterativeCopy<T>(source, array, arrayIndex, count);
        }

        internal static void IterativeCopy<T>(
            IEnumerable<T> source,
            T[] array,
            int arrayIndex,
            int count)
        {
            foreach (T obj in source)
                array[arrayIndex++] = obj;
        }

        internal static T[] ToArray<T>(IEnumerable<T> source)
        {
            if (source is ICollection<T> objs)
            {
                int count = objs.Count;
                if (count == 0)
                    return Array.Empty<T>();
                T[] array = new T[count];
                objs.CopyTo(array, 0);
                return array;
            }

            LargeArrayBuilder<T> largeArrayBuilder = new LargeArrayBuilder<T>(true);
            largeArrayBuilder.AddRange(source);
            return largeArrayBuilder.ToArray();
        }

        internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
        {
            if (source is ICollection<T> objs)
            {
                int count = objs.Count;
                if (count != 0)
                {
                    T[] array = new T[count];
                    objs.CopyTo(array, 0);
                    length = count;
                    return array;
                }
            }
            else
            {
                using IEnumerator<T> enumerator = source.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    T[] array = new T[4]
                    {
                        enumerator.Current,
                        default,
                        default,
                        default
                    };
                    int num = 1;
                    while (enumerator.MoveNext())
                    {
                        if (num == array.Length)
                        {
                            int newSize = num << 1;
                            if ((uint)newSize > 2146435071U)
                                newSize = 2146435071 <= num ? num + 1 : 2146435071;
                            Array.Resize<T>(ref array, newSize);
                        }

                        array[num++] = enumerator.Current;
                    }

                    length = num;
                    return array;
                }
            }

            length = 0;
            return Array.Empty<T>();
        }
    }
}
