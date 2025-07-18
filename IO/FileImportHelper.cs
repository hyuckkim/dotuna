using System.IO;
using System.Threading.Tasks;

namespace DoTuna
{
    public class FileImportHelper
    {
        public string SourcePath { get; }

        public FileImportHelper(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public async Task<string> ReadTextAsync(string relativePath)
        {
            var fullPath = Path.Combine(SourcePath, relativePath);
            return File.Exists(fullPath)
                ? await Task.Run(() => File.ReadAllText(fullPath))
                : string.Empty;
        }

        public async Task<byte[]> ReadBytesAsync(string relativePath)
        {
            var fullPath = Path.Combine(SourcePath, relativePath);
            return File.Exists(fullPath)
                ? await Task.Run(() => File.ReadAllBytes(fullPath))
                : new byte[0];
        }

        public bool FileExists(string relativePath)
        {
            return File.Exists(Path.Combine(SourcePath, relativePath));
        }
    }
}
