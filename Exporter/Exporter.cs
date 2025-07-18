using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoTuna
{
    public class Exporter
    {
        private FileImportHelper ImportHelper { get; set; }
        private FileExportHelper ExportHelper { get; set; } = null!;

        private IProgress<string>? _progress;
        private ThreadFileNameMap _fileNameMap = null!;
        private ScribanRenderer _renderer = null!;
        private List<JsonIndexDocument> _threads = null!;
        private ImageProvider _imageProvider = null!;

        public Exporter(string sourcePath, string resultPath)
        {
            ImportHelper = new FileImportHelper(sourcePath);
            ExportHelper = new FileExportHelper(resultPath);
        }

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            _progress = progress;
            _threads = threads;
            _fileNameMap = new ThreadFileNameMap(threads);
            _imageProvider = new ImageProvider(ImportHelper.SourcePath, ExportHelper.OutputPath);
            _renderer = new ScribanRenderer(_fileNameMap, _imageProvider);

            ExportHelper.EnsureDirectory(".");
            await GenerateIndex();
            if (Setting.Instance.UseCssFile) await GenerateCss();
            await GenerateAllThreads();
        }
        private async Task GenerateIndex()
        {
            var existData = ParseIndex(await ImportHelper.ReadTextAsync("index.html"));

            _progress?.Report("(index.html 생성 중)");
            var newDocs = _threads.Select(doc => HtmlIndexDocument.FromJson(doc, _fileNameMap)).ToList();
            var existingDocs = existData.ToList();

            var fileNameSet = new HashSet<string>(newDocs.Select(d => d.file_name));
            var filteredExistDocs = existingDocs.Where(d => !fileNameSet.Contains(d.file_name));

            var mergedList = newDocs.Concat(filteredExistDocs).ToList();

            var indexHtml = await _renderer.RenderIndexPageAsync(mergedList);
            await ExportHelper.WriteTextAsync("index.html", indexHtml);
            _progress?.Report("(index.html 생성됨)");
        }
        
        public static List<HtmlIndexDocument> ParseIndex(string html)
        {
            var match = Regex.Match(html, @"const data = \[(.*?)\];", RegexOptions.Singleline);
            if (!match.Success)
            {
                return new List<HtmlIndexDocument>();
            }

            string dataContent = match.Groups[1].Value;

            // 1. 속성명에 큰따옴표 씌우기 (key: -> "key":)
            string jsonContent = Regex.Replace(dataContent, @"(\w+):", @"""$1"":");

            // 2. 문자열 값 안전하게 escape 처리
            // 문자열 값은 "..."로 되어 있다고 가정
            jsonContent = Regex.Replace(jsonContent, @"""([^""]*?)""", m => {
                string s = m.Value; // ex: "some "text" here"
                // 내부 큰따옴표는 \"로, 역슬래시는 \\로, 줄바꿈은 \n으로 변환
                string inner = s.Substring(1, s.Length - 2);
                inner = inner.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");
                return $"\"{inner}\"";
            });

            // 3. 마지막 쉼표 제거
            jsonContent = "[" + jsonContent.Trim().TrimEnd(',') + "]";

            // 4. JSON 파싱
            return JsonConvert.DeserializeObject<List<HtmlIndexDocument>>(jsonContent) ?? new List<HtmlIndexDocument>();
        }
        private async Task GenerateCss()
        {
            _progress?.Report("(thread.css 생성 중)");
            string cssContent = CssManager.Instance.GetContent();
            await ExportHelper.WriteTextAsync("thread.css", cssContent);
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
            var name = _fileNameMap.Get(doc.threadId);
            JsonThreadDocument content = JsonThreadDocument.GetThread(
                await ImportHelper.ReadTextAsync($"{doc.threadId}.json")
            );

            var threadHtml = await _renderer.RenderThreadPageAsync(content);
            await ExportHelper.WriteTextAsync(name.FileName, threadHtml);

            if (!Setting.Instance.SingleHTML)
            {
                ExportHelper.EnsureDirectory(Path.Combine(name.PathName, "data"));
                _imageProvider.CopyRequiredImages(content.responses
                .Where(res => !string.IsNullOrEmpty(res.attachment))
                .Select(res => res.attachment)
                .ToList(),
                name);
            }
        }

        private void ReportCount(int count)
        {
            _progress?.Report($"({count} of {_threads.Count})");
        }
    }
}
