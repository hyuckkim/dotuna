using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoTuna
{
    public static class ThreadManager
    {
        public static bool SomethingSelected =>
            Index != null && Index.Any(doc => doc.IsCheck);

        public static List<JsonIndexDocument> Index { get; private set; } = new List<JsonIndexDocument>();

        public static void Open(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            try
            {
                var jsonText = File.ReadAllText(Path.Combine(path, "index.json"));
                var deSerialized = JsonSerializer.Deserialize<List<JsonIndexDocument>>(jsonText);

                Index = deSerialized?.OrderBy(x => x.threadId).ToList()
                         ?? new List<JsonIndexDocument>();
            }
            catch (JsonException e)
            {
                throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
            }
        }

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
