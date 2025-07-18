using System;
using System.IO;
using System.Threading.Tasks;

namespace DoTuna
{
    public class FileHelper: IFileHelper
    {
        public string BasePath { get; }

        public FileHelper(string basePath)
        {
            BasePath = basePath;
        }

        // -----------------------
        // 인스턴스 메서드
        // -----------------------

        public Task<string> ReadTextAsync(string relativePath)
            => ReadTextAsync(BasePath, relativePath);

        public Task<byte[]> ReadBytesAsync(string relativePath)
            => ReadBytesAsync(BasePath, relativePath);

        public Task WriteTextAsync(string relativePath, string content)
            => WriteTextAsync(BasePath, relativePath, content);

        public void CopyFrom(FileHelper sourceHelper, string relativePath)
            => Copy(sourceHelper.BasePath, BasePath, relativePath);

        public bool FileExists(string relativePath)
            => FileExists(BasePath, relativePath);

        public void EnsureDirectory(string relativeDir)
            => EnsureDirectory(BasePath, relativeDir);

        // -----------------------
        // 정적 메서드
        // -----------------------

        public static async Task<string> ReadTextAsync(string basePath, string relativePath)
        {
            var fullPath = Path.Combine(basePath, relativePath);
            return File.Exists(fullPath)
                ? await Task.Run(() => File.ReadAllText(fullPath))
                : string.Empty;
        }

        public static async Task<byte[]> ReadBytesAsync(string basePath, string relativePath)
        {
            var fullPath = Path.Combine(basePath, relativePath);
            return File.Exists(fullPath)
                ? await Task.Run(() => File.ReadAllBytes(fullPath))
                : Array.Empty<byte>();
        }

        public static async Task WriteTextAsync(string basePath, string relativePath, string content)
        {
            if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(content)) return;

            var fullPath = Path.Combine(basePath, relativePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await Task.Run(() => File.WriteAllText(fullPath, content));
        }

        public static void Copy(string sourceBasePath, string destBasePath, string relativePath)
        {
            var source = Path.Combine(sourceBasePath, relativePath);
            var dest = Path.Combine(destBasePath, relativePath);

            var destDir = Path.GetDirectoryName(dest);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            File.Copy(source, dest, overwrite: true);
        }

        public static bool FileExists(string basePath, string relativePath)
        {
            return File.Exists(Path.Combine(basePath, relativePath));
        }

        public static void EnsureDirectory(string basePath, string relativeDir)
        {
            var dirPath = Path.Combine(basePath, relativeDir);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
    }
}
