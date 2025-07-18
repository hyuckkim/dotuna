using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DoTuna.Tests
{
    // 가짜 FileHelper
    public class FakeFileHelper : IFileHelper
    {
        public string BasePath { get; set; } = "";

        private readonly HashSet<string> _existingFiles = new HashSet<string>();
        private readonly Dictionary<string, byte[]> _fileContents = new Dictionary<string, byte[]>();

        public void AddFile(string path, byte[] content)
        {
            string normalized = path.Replace('\\', '/');
            _existingFiles.Add(normalized);
            _fileContents[normalized] = content;
        }
        public bool FileExists(string relativePath)
        {
            string normalized = relativePath.Replace('\\', '/');
            return _existingFiles.Contains(normalized);
        }

        public Task<string> ReadTextAsync(string relativePath)
        {
            return _fileContents.TryGetValue(relativePath, out var bytes)
                ? Task.FromResult(System.Text.Encoding.UTF8.GetString(bytes))
                : Task.FromResult(string.Empty);
        }

        public Task<byte[]> ReadBytesAsync(string relativePath)
        {
            if (_fileContents.TryGetValue(relativePath, out var bytes))
                return Task.FromResult(bytes);
            throw new FileNotFoundException();
        }

        public void EnsureDirectory(string relativePath) { }

        public Task WriteTextAsync(string relativePath, string content) => Task.CompletedTask;

        public Task WriteBytesAsync(string relativePath, byte[] content) => Task.CompletedTask;
    }
}
