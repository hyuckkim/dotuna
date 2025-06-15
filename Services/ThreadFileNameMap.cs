using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DoTuna
{
    public class ThreadFileNameMap
    {
        private readonly Dictionary<string, string> _map;
        public int Size { get => _map.Values.Distinct().Count(); }

        public ThreadFileNameMap(IEnumerable<JsonIndexDocument> threads, string titleTemplate)
        {
            _map = threads.ToDictionary(
                doc => doc.threadId.ToString(),
                doc => GetTemplateName(doc, titleTemplate) + ".html"
            );
        }

        public string GetFileName(string threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f : threadId + ".html";
        }

        public string this[string threadId] => GetFileName(threadId);

        public IReadOnlyDictionary<string, string> AsDictionary() => _map;
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
