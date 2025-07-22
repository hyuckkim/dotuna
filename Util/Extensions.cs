using System;
using System.Collections.Generic;

namespace DoTuna.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipLastOne<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext()) yield break;
                var prev = e.Current;
                while (e.MoveNext())
                {
                    yield return prev;
                    prev = e.Current;
                }
            }
        }
    }
}