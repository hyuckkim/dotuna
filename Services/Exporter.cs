using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DoTuna
{
    public class Exporter
    {
        public string SourcePath { get; set; } = string.Empty;
        public string ResultPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "result");
        public string TitleTemplate { get; set; } = "{id}";

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            var fileNameMap = new ThreadFileNameMap(threads, TitleTemplate);
            var imageCopier = new ImageCopier(SourcePath, ResultPath);
            var renderer = new ScribanRenderer(fileNameMap);

            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);
            string indexPath = Path.Combine(ResultPath, "index.html");

            progress?.Report("(index.html 생성 중)");
            var indexHtml = await renderer.RenderIndexPageAsync(threads);
            await Task.Run(() => File.WriteAllText(indexPath, indexHtml));
            progress?.Report("(index.html 생성됨)");

            int completed = 0;
            progress?.Report($"({completed} of {threads.Count})");

            foreach (var doc in threads)
            {
                var threadPath = Path.Combine(SourcePath, $"{doc.threadId}.json");
                JsonThreadDocument content = await JsonThreadDocument.GetThreadAsync(threadPath);

                string jsonPath = Path.Combine(ResultPath, fileNameMap[doc.threadId.ToString()]);
                var threadHtml = await renderer.RenderThreadPageAsync(content);
                await Task.Run(() => File.WriteAllText(jsonPath, threadHtml));

                imageCopier.CopyRequiredImages(content.responses
                    .Select(res => res.attachment)
                    .Where(img => !string.IsNullOrEmpty(img))
                    .ToList()
                );
                Interlocked.Increment(ref completed);
                progress?.Report($"({completed} of {threads.Count})");
            }
        }
    }
}
