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

        private SynchronizationContext _syncContext = null!;
        private IProgress<string>? _progress;
        private ThreadFileNameMap _fileNameMap = null!;
        private ScribanRenderer _renderer = null!;
        private List<JsonIndexDocument> _threads = null!;
        private ImageCopier _imageCopier = null!;

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            _syncContext = SynchronizationContext.Current;
            _progress = progress;
            _threads = threads;
            _fileNameMap = new ThreadFileNameMap(threads, TitleTemplate);
            _renderer = new ScribanRenderer(_fileNameMap);
            _imageCopier = new ImageCopier(SourcePath, ResultPath);

            EnsurePath();
            await GenerateIndex();
            await GenerateAllThreads();
        }
        private void EnsurePath()
        {
            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);
        }
        private async Task GenerateIndex()
        {
            string indexPath = Path.Combine(ResultPath, "index.html");

            _progress?.Report("(index.html 생성 중)");
            var indexHtml = await _renderer.RenderIndexPageAsync(_threads);
            await Task.Run(() => File.WriteAllText(indexPath, indexHtml));
            _progress?.Report("(index.html 생성됨)");
        }
        private async Task GenerateAllThreads(int maxDegreeOfParallelism = 4)
        {
            int completed = 0;
            ReportCount(0);

            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            var tasks = _threads.Select(async doc =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await GenerateThread(doc);
                    Interlocked.Increment(ref completed);

                    // UI 스레드에서 호출하기 위해 Invoke 사용 (WinForms 기준)
                    ReportCount(completed);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
        
        private async Task GenerateThread(JsonIndexDocument doc)
        {
            string threadPath = Path.Combine(SourcePath, $"{doc.threadId}.json");
            JsonThreadDocument content = await JsonThreadDocument.GetThreadAsync(threadPath);

            string jsonPath = Path.Combine(ResultPath, _fileNameMap[doc.threadId.ToString()]);
            var threadHtml = await _renderer.RenderThreadPageAsync(content);
            await Task.Run(() => File.WriteAllText(jsonPath, threadHtml));

            _imageCopier.CopyRequiredImages(content.responses
                .Select(res => res.attachment)
                .Where(img => !string.IsNullOrEmpty(img))
                .ToList()
            );
        }

        private void ReportCount(int count)
        {
            if (_syncContext != null)
            {
                _syncContext.Post(_ => _progress?.Report($"({count} of {_threads.Count})"), null);
            }
            else
            {
                _progress?.Report($"({count} of {_threads.Count})");
            }
        }
    }
}
