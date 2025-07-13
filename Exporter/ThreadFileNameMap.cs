using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DoTuna
{

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
