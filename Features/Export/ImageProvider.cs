using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DoTuna.Features.Export
{
    public class ImageProvider
    {
        private readonly IFileHelper _sourceHelper;
        private readonly IFileHelper _resultHelper;

        public ImageProvider(IFileHelper sourceHelper, IFileHelper resultHelper)
        {
            _sourceHelper = sourceHelper;
            _resultHelper = resultHelper;
        }

        public void CopyRequiredImages(List<string> requireImg, ThreadFileName thread)
        {
            if (requireImg.Count == 0) return;

            // data 디렉토리 경로
            string dataDir = Path.Combine(_resultHelper.BasePath, thread.PathName, "data");
            _resultHelper.EnsureDirectory(Path.Combine(thread.PathName, "data"));

            Parallel.ForEach(requireImg, imgFile =>
            {
                var src = Path.Combine(_sourceHelper.BasePath, "data", imgFile);
                var dst = Path.Combine(dataDir, imgFile);
                if (_sourceHelper.FileExists(Path.Combine("data", imgFile)))
                {
                    File.Copy(src, dst, true);
                }
            });
        }

        public async Task<string> Href(string fileName)
        {
            if (Setting.Instance.SingleHTML)
                return await GetDataHref(fileName);
            else
                return "data/" + Uri.EscapeDataString(fileName);
        }

        private async Task<string> GetDataHref(string fileName)
        {
            if (!_sourceHelper.FileExists(Path.Combine("data", fileName)))
                return Uri.EscapeDataString(fileName);

            byte[] imageBytes = await _sourceHelper.ReadBytesAsync(Path.Combine("data", fileName));
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
