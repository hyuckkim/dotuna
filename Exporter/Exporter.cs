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
        private FileHelper _sourceHelper = null!;
        private FileHelper _resultHelper = null!;

        private IProgress<string>? _progress;
        private ThreadFileNameMap _fileNameMap = null!;
        private ScribanRenderer _renderer = null!;
        private List<JsonIndexDocument> _threads = null!;
        private ImageProvider _imageProvider = null!;

        // 생성자 대신 초기화 메서드로 변경 (원하면 생성자로 변경 가능)
        public Exporter(string sourcePath, string resultPath)
        {
            _sourceHelper = new FileHelper(sourcePath);
            _resultHelper = new FileHelper(resultPath);
        }

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            _progress = progress;
            _threads = threads;
            _fileNameMap = new ThreadFileNameMap(threads);
            _imageProvider = new ImageProvider(_sourceHelper, _resultHelper);
            _renderer = new ScribanRenderer(_fileNameMap, _imageProvider);

            FileHelper.EnsureDirectory(_resultHelper.BasePath, ""); // EnsurePath -> EnsureDirectory로 수정

            await GenerateIndex();
            if (Setting.Instance.UseCssFile) await GenerateCss();
            await GenerateAllThreads();
        }

        private async Task GenerateIndex()
        {
            string indexFile = "index.html";
            string indexPath = Path.Combine(_resultHelper.BasePath, indexFile);

            var existData = await ParseIndex(indexPath);

            _progress?.Report("(index.html 생성 중)");

            var newDocs = _threads.Select(doc => HtmlIndexDocument.FromJson(doc, _fileNameMap)).ToList();
            var existingDocs = existData.ToList();

            var fileNameSet = new HashSet<string>(newDocs.Select(d => d.file_name));
            var filteredExistDocs = existingDocs.Where(d => !fileNameSet.Contains(d.file_name));

            var mergedList = newDocs.Concat(filteredExistDocs).ToList();

            var indexHtml = await _renderer.RenderIndexPageAsync(mergedList);
            await _resultHelper.WriteTextAsync(indexFile, indexHtml);

            _progress?.Report("(index.html 생성됨)");
        }
        public async Task<List<HtmlIndexDocument>> ParseIndex(string path)
        {
            if (string.IsNullOrEmpty(path) || !_sourceHelper.FileExists(path))
            {
                return new List<HtmlIndexDocument>();
            }
            var html = await _sourceHelper.ReadTextAsync(path);
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
            string cssFile = "thread.css";

            _progress?.Report("(thread.css 생성 중)");
            string cssContent = CssManager.Instance.GetContent();
            await _resultHelper.WriteTextAsync(cssFile, cssContent);
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
            string threadJsonFile = $"{doc.threadId}.json";

            var threadContent = await JsonThreadDocument.GetThread(
                Path.Combine(_sourceHelper.BasePath, threadJsonFile));

            string threadHtmlFile = _fileNameMap[doc.threadId];
            var threadHtml = await _renderer.RenderThreadPageAsync(threadContent);
            await _resultHelper.WriteTextAsync(threadHtmlFile, threadHtml);

            if (!Setting.Instance.SingleHTML)
            {
                var attachments = threadContent.responses
                    .Where(res => !string.IsNullOrEmpty(res.attachment))
                    .Select(res => res.attachment)
                    .ToList();

                _imageProvider.CopyRequiredImages(attachments, _fileNameMap.Get(doc.threadId));
            }
        }

        private void ReportCount(int count)
        {
            _progress?.Report($"({count} of {_threads.Count})");
        }
    }
}
