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
            if (Setting.Instance.SingleHTML) return Path.Combine("data", Uri.EscapeDataString(fileName));
            else return Path.Combine("data", Uri.EscapeDataString(fileName));
        }
    }
}
