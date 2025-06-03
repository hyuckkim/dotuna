using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DoTuna
{
    public class IndexFileRepository : IIndexRepository
    {
        private List<JsonIndexDocument> _documents = new List<JsonIndexDocument>();
        public List<JsonIndexDocument> Get() => _documents;

        public void Open(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            try
            {
                var jsonText = File.ReadAllText(Path.Combine(path, "index.json"));
                var deSerialized = JsonSerializer.Deserialize<List<JsonIndexDocument>>(jsonText);

                _documents = deSerialized?.OrderBy(x => x.threadId).ToList()
                    ?? new List<JsonIndexDocument>();
            }
            catch (JsonException e)
            {
                throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
            }
        }
        public async Task OpenAsync(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            try
            {
                using (var stream = File.OpenRead(Path.Combine(path, "index.json")))
                {
                    var deSerialized = await JsonSerializer.DeserializeAsync<List<JsonIndexDocument>>(stream);

                    _documents = deSerialized?.OrderBy(x => x.threadId).ToList()
                        ?? new List<JsonIndexDocument>();
                }
            }
            catch (JsonException e)
            {
                throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
            }
        }
    }
}