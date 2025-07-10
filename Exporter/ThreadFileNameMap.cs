using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace DoTuna
{
    public class ThreadFileName
    {
        public string ThreadId { get; }
        public string[] FileNameSegments { get; }
        public string FileName => string.Join("/", FileNameSegments);
        public ThreadFileName(JsonIndexDocument doc, string pattern)
        {
            ThreadId = doc.threadId.ToString();
            var fileName = ThreadFileNameMap.GetTemplateName(doc, pattern) + ".html";
            FileNameSegments = fileName.Split('/');
        }
        public string GetRelativePathTo(ThreadFileName target)
        {
            var from = this.FileNameSegments;
            var to = target.FileNameSegments;
            int len = Math.Min(from.Length, to.Length);
            int common = 0;
            while (common < len && from[common] == to[common]) common++;
            var up = Enumerable.Repeat("..", from.Length - common - 1); // -1: 파일명 제외
            var down = to.Skip(common);
            var rel = up.Concat(down);
            return string.Join("/", rel);
        }
        public string GetRelativePathToRoot()
        {
            if (FileNameSegments.Length <= 1) return "";
            return string.Join("/", Enumerable.Repeat("..", FileNameSegments.Length - 1));
        }
    }

    public class ThreadFileNameMap
    {
        private readonly Dictionary<string, ThreadFileName> _map;
        public int FileNameCount { get => _map.Values.Select(x => x.FileName).Distinct().Count(); }

        public ThreadFileNameMap(IEnumerable<JsonIndexDocument> threads)
        {
            _map = threads.ToDictionary(
                doc => doc.threadId.ToString(),
                doc => new ThreadFileName(doc, Setting.Instance.Pattern)
            );
        }

        public string GetFileName(string threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f.FileName : threadId + ".html";
        }

        public ThreadFileName Get(string threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f : new ThreadFileName(
                new JsonIndexDocument { threadId = int.Parse(threadId) }, Setting.Instance.Pattern);
        }

        public string this[string threadId] => GetFileName(threadId);
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

            var invalidChars = Path.GetInvalidFileNameChars();
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
