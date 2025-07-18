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
            if (!FileHelper.FileExists(path, "index.json"))
                throw new DirectoryNotFoundException($"Directory or file not found: {path}\\index.json");

            var jsonText = await FileHelper.ReadTextAsync(path, "index.json");
            var deSerialized = JsonConvert.DeserializeObject<List<JsonIndexDocument>>(jsonText);

            _documents = deSerialized?.OrderBy(x => x.threadId).ToList()
                ?? new List<JsonIndexDocument>();
        }
    }
}
