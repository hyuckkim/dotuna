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


        public async Task Open(string path)
        {
            if (!FileHelper.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            var jsonText = await FileHelper.ReadAllTextAsync(Path.Combine(path, "index.json"));
            var deSerialized = JsonConvert.DeserializeObject<List<JsonIndexDocument>>(jsonText);

            _documents = deSerialized?.OrderBy(x => x.threadId).ToList()
                ?? new List<JsonIndexDocument>();
        }
    }
}
