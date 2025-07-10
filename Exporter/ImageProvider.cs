using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DoTuna
{
    public class ImageProvider
    {
        private readonly string _sourcePath;
        private readonly string _resultPath;

        public ImageProvider(string sourcePath, string resultPath)
        {
            _sourcePath = sourcePath;
            _resultPath = resultPath;
        }

        public void CopyRequiredImages(List<string> requireImg)
        {
            if (requireImg.Count == 0) return;

            string dataDir = Path.Combine(_resultPath, "data");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            Parallel.ForEach(requireImg, imgFile =>
            {
                var src = Path.Combine(_sourcePath, "data", imgFile);
                var dst = Path.Combine(dataDir, imgFile);
                if (File.Exists(src))
                {
                    File.Copy(src, dst, true);
                }
            });
        }

        public async Task<string> Href(string fileName)
        {
            if (Setting.Instance.SingleHTML) return await GetDataHref(fileName);
            else return Path.Combine("data", Uri.EscapeDataString(fileName));
        }

        private async Task<string> GetDataHref(string fileName)
        {
            string filePath = Path.Combine(_sourcePath, "data", fileName);
            if (!File.Exists(filePath))
                return Uri.EscapeDataString(fileName);

            byte[] imageBytes = await FileHelper.ReadAllByteAsync(filePath);
            string mimeType = GetMimeType(fileName);
            string base64Image = Convert.ToBase64String(imageBytes);
            return $"data:{mimeType};base64,{base64Image}";
        }
        private static string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
