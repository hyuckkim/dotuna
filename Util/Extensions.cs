using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    public static class PathExtensions
    {
        public static string ReplaceInvalidFileNameChars(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var invalidChars = Path.GetInvalidFileNameChars()
                .Where(c => c != '/' && c != '\\') // Allow slashes for relative paths
                .ToArray();
            foreach (var c in invalidChars)
            {
                input = input.Replace(c.ToString(), "");
            }
            return input;
        }

        public static string Truncate(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength);
        }
    }
}