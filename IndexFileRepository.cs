
public class IndexFileRepository : IIndexRepository
{
    private List<JsonThreadDocument> _documents;
    public IEnumerable<JsonThreadDocument> Get() => _documents;

    public Open(string path)
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
}