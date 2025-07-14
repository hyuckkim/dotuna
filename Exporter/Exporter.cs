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

        private IProgress<string>? _progress;
        private ThreadFileNameMap _fileNameMap = null!;
        private ScribanRenderer _renderer = null!;
        private List<JsonIndexDocument> _threads = null!;
        private ImageProvider _imageProvider = null!;

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            _progress = progress;
            _threads = threads;
            _fileNameMap = new ThreadFileNameMap(threads);
            _imageProvider = new ImageProvider(SourcePath, ResultPath);
            _renderer = new ScribanRenderer(_fileNameMap, _imageProvider);

            FileHelper.EnsurePath(ResultPath);
            await GenerateIndex();
            if (Setting.Instance.UseCssFile) await GenerateCss();
            await GenerateAllThreads();
        }
        private async Task GenerateIndex()
        {
            string indexPath = Path.Combine(ResultPath, "index.html");
            var existData = await IndexParser.ParseIndex(indexPath);

            _progress?.Report("(index.html 생성 중)");
            var indexHtml = await _renderer.RenderIndexPageAsync(
                _threads
                    .Select(doc => HtmlIndexDocument.FromJson(doc, _fileNameMap))
                    .Union(existData).ToList()
            );
            await FileHelper.WriteAllTextAsync(indexPath, indexHtml);
            _progress?.Report("(index.html 생성됨)");
        }
        private async Task GenerateCss()
        {
            string CssPath = Path.Combine(ResultPath, "thread.css");

            _progress?.Report("(thread.css 생성 중)");
            string cssContent = CssManager.Instance.GetContent();
            await FileHelper.WriteAllTextAsync(CssPath, cssContent);
            _progress?.Report("(thread.css 생성됨)");
        }
        private async Task GenerateAllThreads()
        {
            int completed = 0;
            ReportCount(0);

            var semaphore = new SemaphoreSlim(Setting.Instance.ThreadCount);

            var tasks = new List<Task>();

            foreach (var doc in _threads)
            {
                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await GenerateThread(doc);
                        Interlocked.Increment(ref completed);
                        ReportCount(completed);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
        private async Task GenerateThread(JsonIndexDocument doc)
        {
            string threadPath = Path.Combine(SourcePath, $"{doc.threadId}.json");
            JsonThreadDocument content = await JsonThreadDocument.GetThreadAsync(threadPath);

            string jsonPath = Path.Combine(ResultPath, _fileNameMap[doc.threadId]);
            var threadHtml = await _renderer.RenderThreadPageAsync(content);
            await FileHelper.WriteAllTextAsync(jsonPath, threadHtml);

            if (!Setting.Instance.SingleHTML) _imageProvider.CopyRequiredImages(content.responses
                .Where(res => !string.IsNullOrEmpty(res.attachment))
                .Select(res => res.attachment)
                .ToList(),
                _fileNameMap.Get(doc.threadId)
            );
        }

        private void ReportCount(int count)
        {
            _progress?.Report($"({count} of {_threads.Count})");
        }
    }
}
