using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace DoTuna
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
    public class ThreadFileName : IEquatable<ThreadFileName>
    {
        public ulong ThreadId { get; }
        public string[] FileNameSegments { get; }
        public string FileName => string.Join("/", FileNameSegments);
        public string PathName => string.Join("/", FileNameSegments.SkipLastOne());

        public ThreadFileName(JsonIndexDocument doc, string pattern)
        {
            ThreadId = doc.threadId;
            var fileName = ThreadFileNameMap.GetTemplateName(doc, pattern) + ".html";
            FileNameSegments = fileName.Split('/');
        }

        public ThreadFileName(ulong threadId, string[] fileNameSegments)
        {
            ThreadId = threadId;
            FileNameSegments = fileNameSegments ?? throw new ArgumentNullException(nameof(fileNameSegments));
        }

        public string GetRelativePathTo(ThreadFileName target)
        {
            var from = this.FileNameSegments;
            var to = target.FileNameSegments;
            if (from == to) return target.FileName;
            int len = Math.Min(from.Length, to.Length);
            int common = 0;
            while (common < len && from[common] == to[common]) common++;
            var upCount = Math.Max(0, from.Length - common - 1);
            var up = Enumerable.Repeat("..", upCount);
            var down = to.Skip(common);
            var rel = up.Concat(down);
            return string.Join("/", rel);
        }

        public string GetRelativePathToRoot()
        {
            if (FileNameSegments.Length <= 1) return "";
            return string.Join("/", Enumerable.Repeat("..", FileNameSegments.Length - 1));
        }

        public override bool Equals(object obj)
        {
            if (obj is ThreadFileName o)
            {
                return Equals(o);
            }
            else return false;
        }

        public bool Equals(ThreadFileName other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ThreadId != other.ThreadId) return false;
            if (FileNameSegments.Length != other.FileNameSegments.Length) return false;
            for (int i = 0; i < FileNameSegments.Length; i++)
            {
                if (!string.Equals(FileNameSegments[i], other.FileNameSegments[i], StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ThreadId.GetHashCode();
                foreach (var segment in FileNameSegments)
                {
                    hash = (hash * 397) ^ (segment != null ? segment.GetHashCode() : 0);
                }
                return hash;
            }
        }

        public static bool operator ==(ThreadFileName left, ThreadFileName right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ThreadFileName left, ThreadFileName right)
        {
            return !(left == right);
        }
    }

    public class ThreadFileNameMap
    {
        private readonly Dictionary<ulong, ThreadFileName> _map;
        public int FileNameCount { get => _map.Values.Select(x => x.FileName).Distinct().Count(); }

        public ThreadFileNameMap(IEnumerable<JsonIndexDocument> threads)
        {
            _map = threads.ToDictionary(
                doc => doc.threadId,
                doc => new ThreadFileName(doc, Setting.Instance.Pattern)
            );
        }

        public string GetFileName(ulong threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f.FileName : threadId + ".html";
        }

        public ThreadFileName Get(ulong threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f : new ThreadFileName(
                new JsonIndexDocument { threadId = threadId }, Setting.Instance.Pattern);
        }

        public string this[ulong threadId] => GetFileName(threadId);
        public static string GetTemplateName(JsonIndexDocument doc, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var values = new Dictionary<string, string>
            {
                { "id", doc.threadId.ToString() },
                { "title", doc.title },
                { "name", doc.username },
                { "created", doc.createdAt.ToString("yyyy-MM-dd") },
                { "updated", doc.updatedAt.ToString("yyyy-MM-dd") },
                { "size", doc.size.ToString() }
            };

            return TemplateFormatter
                .Format(template, values)
                .ReplaceInvalidFileNameChars()
                .Truncate(200);
        }
    }
    internal static class StringExtensions
    {
        internal static string ReplaceInvalidFileNameChars(this string input)
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

        internal static string Truncate(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength);
        }
    }
}
