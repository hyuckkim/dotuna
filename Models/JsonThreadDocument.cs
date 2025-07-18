using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoTuna
{
    public class Response
    {
#pragma warning disable IDE1006 // Naming Styles: Json has camelCase but c# is PascalCase
        public ulong threadId { get; set; } = 0;
        public int sequence { get; set; } = 0;
        public string username { get; set; } = "";
        public string userId { get; set; } = "";
        public DateTime createdAt { get; set; } = new DateTime(0);
        public string content { get; set; } = "";
        public string attachment { get; set; } = "";
#pragma warning restore IDE1006 // Naming Styles
    }

    public class JsonThreadDocument
    {
#pragma warning disable IDE1006 // Naming Styles: Json has camelCase but c# is PascalCase
        public string version { get; set; } = "";
        public string boardId { get; set; } = "";
        public ulong threadId { get; set; } = 0;
        public string title { get; set; } = "";
        public string username { get; set; } = "";
        public DateTime createdAt { get; set; } = new DateTime(0);
        public DateTime updatedAt { get; set; } = new DateTime(0);
        public int size { get; set; } = 0;
        public List<Response> responses { get; set; } = new List<Response>();
#pragma warning restore IDE1006 // Naming Styles

        public static JsonThreadDocument GetThread(string json)
        {
            return JsonConvert.DeserializeObject<JsonThreadDocument>(json)
                ?? throw new JsonException($"Failed to parse thread JSON file: {json}");
        }
    }
}
