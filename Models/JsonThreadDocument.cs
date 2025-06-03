using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoTuna
{
    public class Response
    {
#pragma warning disable IDE1006 // Naming Styles: Json has camelCase but c# is PascalCase
        public int threadId { get; set; } = 0;
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
        public int threadId { get; set; } = 0;
        public string title { get; set; } = "";
        public string username { get; set; } = "";
        public DateTime createdAt { get; set; } = new DateTime(0);
        public DateTime updatedAt { get; set; } = new DateTime(0);
        public int size { get; set; } = 0;
        public List<Response> responses { get; set; } = new List<Response>();
#pragma warning restore IDE1006 // Naming Styles

        public static JsonThreadDocument GetThread(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Thread file not found: {path}");

            var jsonText = File.ReadAllText(path);
            return JsonSerializer.Deserialize<JsonThreadDocument>(jsonText)
                ?? throw new JsonException($"Failed to parse thread JSON file: {path}");
        }

        public static async Task<JsonThreadDocument> GetThreadAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Thread file not found: {path}");

            using var stream = File.OpenRead(path);
            var doc = await JsonSerializer.DeserializeAsync<JsonThreadDocument>(stream);
            return doc ?? throw new JsonException($"Failed to parse thread JSON file: {path}");
        }
    }
}
