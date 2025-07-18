using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DoTuna.Test
{
    // 가짜 FileHelper
    public class FakeFileHelper : IFileHelper
    {
        public string BasePath { get; set; } = "";

        private readonly HashSet<string> _existingFiles = new HashSet<string>();
        private readonly Dictionary<string, byte[]> _fileContents = new Dictionary<string, byte[]>();

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimStart('/');
        }

        public void AddFile(string path, byte[] content)
        {
            var normalized = NormalizePath(path);
            _existingFiles.Add(normalized);
            _fileContents[normalized] = content;
        }

        public bool FileExists(string relativePath) => 
            _existingFiles.Contains(NormalizePath(relativePath));

        public Task<byte[]> ReadBytesAsync(string relativePath)
        {
            var normalized = NormalizePath(relativePath);
            if (_fileContents.TryGetValue(normalized, out var bytes))
                return Task.FromResult(bytes);
            throw new FileNotFoundException();
        }

        public Task<string> ReadTextAsync(string relativePath)
        {
            var normalized = NormalizePath(relativePath);
            if (_fileContents.TryGetValue(normalized, out var bytes))
                return Task.FromResult(System.Text.Encoding.UTF8.GetString(bytes));
            throw new FileNotFoundException();
        }

        public void EnsureDirectory(string relativePath) { }

        public Task WriteTextAsync(string relativePath, string content) => Task.CompletedTask;

        public Task WriteBytesAsync(string relativePath, byte[] content) => Task.CompletedTask;
    }
}
