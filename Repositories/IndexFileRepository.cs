using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
                var deSerialized = JsonConvert.DeserializeObject<List<JsonIndexDocument>>(jsonText);

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

            var filePath = Path.Combine(path, "index.json");

            try
            {
                string jsonText;
                using (var reader = new StreamReader(filePath))
                {
                    jsonText = await reader.ReadToEndAsync();
                }

                var deSerialized = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<JsonIndexDocument>>(jsonText));

                _documents = deSerialized ?? new List<JsonIndexDocument>();
            }
            catch (JsonException e)
            {
                throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
            }
        }
    }
}
