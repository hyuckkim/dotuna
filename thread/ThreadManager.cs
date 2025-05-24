using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Data.DataSetExtensions;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoTuna.Thread
{
    public static class ThreadManager
    {
        public static string FilePath { get; set; } = String.Empty;
        public static bool SomethingSelected =>
            Index.AsEnumerable().Any(row => row.Field<bool>("IsCheck")) ?? false;

        public static DataTable Index { get; private set; } = new DataTable();

        public static void Open(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
            FilePath = path;
            try
            {
                var jsonText = File.ReadAllText(Path.Combine(path, "index.json"));
                var deSerialized = JsonSerializer.Deserialize<List<JsonIndexDocument>>(jsonText) ?? new List<JsonIndexDocument>();

                Index = new DataTable();
                Index.Columns.Add("IsCheck", typeof(bool));
                Index.Columns.Add("threadId", typeof(int));
                Index.Columns.Add("title", typeof(string));
                Index.Columns.Add("username", typeof(string));
                foreach (var item in deSerialized.OrderBy(x => x.threadId))
                {
                    Index.Rows.Add(false, item.threadId, item.title, item.username);
                }
            }
            catch (JsonException e)
            {
                throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
            }
        }

        public static JsonThreadDocument GetThread(int id)
        {
            var threadPath = Path.Combine(FilePath, $"{id}.json");
            if (!File.Exists(threadPath))
            {
                throw new FileNotFoundException($"Thread file not found: {threadPath}");
            }
            var jsonText = File.ReadAllText(threadPath);
            return JsonSerializer.Deserialize<JsonThreadDocument>(jsonText)
                ?? throw new JsonException($"Failed to parse thread JSON file: {threadPath}");
        }

        public static async Task<JsonThreadDocument> GetThreadAsync(int id)
        {
            var threadPath = Path.Combine(FilePath, $"{id}.json");
            if (!File.Exists(threadPath))
            {
                throw new FileNotFoundException($"Thread file not found: {threadPath}");
            }

            using (var stream = File.OpenRead(threadPath))
            {
                var doc = await JsonSerializer.DeserializeAsync<JsonThreadDocument>(stream);
                if (doc == null)
                {
                    throw new JsonException($"Failed to parse thread JSON file: {threadPath}");
                }
                return doc;
            }
        }
    }
}
