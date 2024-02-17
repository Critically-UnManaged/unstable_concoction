using System;
using System.Collections.Generic;

namespace UnstableConcoction.Extensions;

public static class GenericExtensions
{
    /// <summary>
    /// Extension for all IComparables, like numbers and dates. Returns true if given data
    /// is between the min and max values. By default, it is inclusive.
    /// </summary>
    /// <param name="value">Value to compare</param>
    /// <param name="min">Minimum value in the range</param>
    /// <param name="max">Maximum value in the range</param>
    /// <param name="inclusive">Changes the behavior for min and max, true by default</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>True if the given value is  between the given range</returns>
    public static bool IsBetween<T>(this T value, T min, T max, bool inclusive=true) where T : IComparable
    {
        return inclusive
            ? Comparer<T>.Default.Compare(value, min) >= 0
              && Comparer<T>.Default.Compare(value, max) <= 0
            : Comparer<T>.Default.Compare(value, min) > 0
              && Comparer<T>.Default.Compare(value, max) < 0;
    }
}