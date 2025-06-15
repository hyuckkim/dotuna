using System;

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
    }
}
