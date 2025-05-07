namespace DoTuna.Thread;

using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class ThreadManager
{
    public static string? FilePath { get; private set; }
    public static bool SomethingSelected { get => Index.Any(x => x.IsCheck);}
    public static ObservableCollection<JsonIndexDocument> Index { get; private set; } = [];
    public static void Open(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }
        FilePath = path;
        try {
            var deSerialized = JsonSerializer.Deserialize<List<JsonIndexDocument>>(File.ReadAllText(Path.Combine(path, "index.json"))) ?? [];
            Index = new ObservableCollection<JsonIndexDocument>(deSerialized.OrderBy(x => x.threadId));
        }
        catch (JsonException e)
        {
            throw new JsonException($"Failed to parse JSON file: {e.Message}", e);
        }
    }
    public static JsonThreadDocument GetThread(int id)
    {
        var threadPath = Path.Combine(FilePath!, $"{id}.json");
        if (!File.Exists(threadPath))
        {
            throw new FileNotFoundException($"Thread file not found: {threadPath}");
        }
        return JsonSerializer.Deserialize<JsonThreadDocument>(File.ReadAllText(threadPath))
            ?? throw new JsonException($"Failed to parse thread JSON file: {threadPath}");
    }

    public static async Task<JsonThreadDocument> GetThreadAsync(int id)
    {
        var threadPath = Path.Combine(FilePath!, $"{id}.json");
        if (!File.Exists(threadPath))
        {
            throw new FileNotFoundException($"Thread file not found: {threadPath}");
        }

        // 비동기로 파일 읽기
        using var stream = File.OpenRead(threadPath);
        return await JsonSerializer.DeserializeAsync<JsonThreadDocument>(stream)
            ?? throw new JsonException($"Failed to parse thread JSON file: {threadPath}");
    }
}