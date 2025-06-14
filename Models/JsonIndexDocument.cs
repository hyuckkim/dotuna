using System;
using System.Collections.Generic;
using System.IO;

namespace DoTuna
{
    public class JsonIndexDocument
    {
#pragma warning disable IDE1006 // Naming Styles: Json has camelCase but c# is PascalCase
        public string version { get; set; } = "";
        public string boardId { get; set; } = "";
        public int threadId { get; set; } = 0;
        public string title { get; set; } = "";
        public string username { get; set; } = "";
        public DateTime createdAt { get; set; } = new DateTime(0);
        public DateTime updatedAt { get; set; } = new DateTime(0);
        public int size { get; set; } = 0;
#pragma warning restore IDE1006 // Naming Styles

        public override string ToString()
        {
            return string.Format("{0} /  Username: {1} {2}", title, username, threadId);
        }

        public string getTemplateName(string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var values = new Dictionary<string, string>
            {
                { "id", this.threadId.ToString() },
                { "title", this.title },
                { "name", this.username },
                { "created", this.createdAt.ToString("yyyy-MM-dd") },
                { "updated", this.updatedAt.ToString("yyyy-MM-dd") },
                { "size", this.size.ToString() }
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
