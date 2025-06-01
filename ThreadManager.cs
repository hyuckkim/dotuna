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
    }
}
