using DoTuna.Export;

public class ExportAppService
{
    public async Task ExportAsync(string pattern, IProgress<string> progress)
    {
        await ExportManager.Build(pattern, progress);
    }
}
