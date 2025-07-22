using System.Threading.Tasks;

namespace DoTuna.Core
{
    public interface IFileHelper
    {
        string BasePath { get; }

        bool FileExists(string relativePath);

        Task<string> ReadTextAsync(string relativePath);
        Task<byte[]> ReadBytesAsync(string relativePath);

        Task WriteTextAsync(string relativePath, string content);

        void EnsureDirectory(string subPath);
    }
}
