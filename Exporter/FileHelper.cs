using System.IO;
using System.Threading.Tasks;

namespace DoTuna
{
    public static class FileHelper
    {
        public static async Task<string> ReadAllTextAsync(string path)
        {
            if (!Exists(path)) return string.Empty;
            return await Task.Run(() => File.ReadAllText(path));
        }
        public static async Task<byte[]> ReadAllByteAsync(string path)
        {
            if (!Exists(path)) return new byte[0];
            return await Task.Run(() => File.ReadAllBytes(path));
        }
        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
