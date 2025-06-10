using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoTuna
{
    public class ImageCopier
    {
        private readonly string _sourcePath;
        private readonly string _resultPath;

        public ImageCopier(string sourcePath, string resultPath)
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

            foreach (var imgFile in requireImg.Distinct())
            {
                var src = Path.Combine(_sourcePath, "data", imgFile);
                var dst = Path.Combine(dataDir, imgFile);
                if (File.Exists(src))
                {
                    File.Copy(src, dst, true);
                }
            }
        }
    }
}
