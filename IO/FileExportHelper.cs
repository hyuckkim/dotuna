using System.IO;
using System.Threading.Tasks;

namespace DoTuna
{
    public class FileExportHelper
    {
        public string OutputPath { get; }

        public FileExportHelper(string outputPath)
        {
            OutputPath = outputPath;
        }

        public async Task WriteTextAsync(string relativePath, string content)
        {
            if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(content)) return;

            var fullPath = Path.Combine(OutputPath, relativePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await Task.Run(() => File.WriteAllText(fullPath, content));
        }

        public bool FileExists(string relativePath)
        {
            return File.Exists(Path.Combine(OutputPath, relativePath));
        }

        public void EnsureDirectory(string relativePath)
        {
            var dirPath = Path.Combine(OutputPath, relativePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
    }
}
