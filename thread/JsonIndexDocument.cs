using System;
using System.Collections.Generic;

namespace DoTuna.Thread
{
    public class JsonIndexDocument
    {
#pragma warning disable IDE1006 // Naming Styles: Because the JSON properties are in snake_case, we need to disable this rule for the properties.
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

        public bool IsCheck { get; set; }

        public string getTemplateName(string template)
        {
            return template
                .Replace("{id}", this.threadId.ToString())
                .Replace("{title}", this.title)
                .Replace("{name}", this.username)
                .Replace("{created}", this.createdAt.ToString("yyyy-MM-dd"))
                .Replace("{updated}", this.updatedAt.ToString("yyyy-MM-dd"))
                .Replace("{size}", this.size.ToString());
        }
    }
}
