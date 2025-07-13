using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DoTuna
{
    public static class FileHelper
    {
        private static readonly Dictionary<string, object> _pathLocks = new Dictionary<string, object>();
        private static readonly object _lockDictLock = new object();

        private static object GetLockForPath(string path)
        {
            lock (_lockDictLock)
            {
                if (!_pathLocks.TryGetValue(path, out var lockObj))
                {
                    lockObj = new object();
                    _pathLocks[path] = lockObj;
                }
                return lockObj;
            }
        }

        public static async Task<string> ReadAllTextAsync(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            return await Task.Run(() => File.ReadAllText(path));
        }
        public static async Task<byte[]> ReadAllByteAsync(string path)
        {
            if (!File.Exists(path)) return new byte[0];
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

            var pathLock = GetLockForPath(path);

            await Task.Run(() =>
            {
                lock (pathLock)
                {
                    File.WriteAllText(path, content);
                }
            });
        }

        public static void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        public static string EncodeToHref(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (IsTargetChar(c))
                    sb.Append($"%{(int)c:X2}");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsTargetChar(char c)
        {
            // 공백
            if (c == ' ') return true;

            // 제어 문자
            if (c == '\n' || c == '\r' || c == '\t') return true;

            // 특수 문자
            char[] specials = { '!', '@', '#', '%', '^', '&', '*', '(', ')' };
            return Array.IndexOf(specials, c) >= 0;
        }
    }
}
