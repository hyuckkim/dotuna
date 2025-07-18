using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DoTuna
{
    public static class FileHelper
    {
        public static async Task<string> ReadAllTextAsync(string path)
        {
            if (!FileHelper.Exists(path)) return string.Empty;
            return await Task.Run(() => File.ReadAllText(path));
        }
        public static async Task<byte[]> ReadAllByteAsync(string path)
        {
            if (!FileHelper.Exists(path)) return new byte[0];
            return await Task.Run(() => File.ReadAllBytes(path));
        }
        public static async Task WriteAllTextAsync(string path, string content)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(content)) return;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsurePath(directory);
            }
            await Task.Run(() => File.WriteAllText(path, content));
        }
        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
        public static void EnsurePath(string path)
        {
            if (!Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
