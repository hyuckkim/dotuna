using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Scriban;

namespace DoTuna
{
    public class Exporter
    {
        public string SourcePath { get; set; } = string.Empty;
        public string ResultPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Result");
        public string TitleTemplate { get; set; } = "{id}";

        private Dictionary<string, string> _threadIdToFileName = new Dictionary<string, string>();

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            _threadIdToFileName = threads.ToDictionary(
                doc => doc.threadId.ToString(),
                doc => doc.getTemplateName(TitleTemplate) + ".html"
            );

            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);
            string indexPath = Path.Combine(ResultPath, "index.html");

            progress?.Report("(index.html 생성 중)");
            var indexHtml = await GenerateIndexPageAsync(threads);
            await Task.Run(() => File.WriteAllText(indexPath, indexHtml));
            progress?.Report("(index.html 생성됨)");

            int completed = 0;
            progress?.Report($"({completed} of {threads.Count})");

            foreach (var doc in threads)
            {
                var threadPath = Path.Combine(SourcePath, $"{doc.threadId}.json");
                JsonThreadDocument content = await JsonThreadDocument.GetThreadAsync(threadPath);

                string jsonPath = Path.Combine(ResultPath, _threadIdToFileName[doc.threadId.ToString()]);
                var threadHtml = await GenerateThreadPage(content);
                await Task.Run(() => File.WriteAllText(jsonPath, threadHtml));

                CopyRequiredImages(content.responses
                    .Select(res => res.attachment)
                    .Where(img => !string.IsNullOrEmpty(img))
                    .ToList()
                );
                Interlocked.Increment(ref completed);
                progress?.Report($"({completed} of {threads.Count})");
            }

        }

        async Task<string> GenerateIndexPageAsync(List<JsonIndexDocument> threads)
        {
            var assembly = typeof(Exporter).Assembly;
            var stream = assembly.GetManifestResourceStream("DoTuna.Templates.index.html");
            var reader = new StreamReader(stream, Encoding.UTF8);
            var templateText = await reader.ReadToEndAsync();
            reader.Dispose();
            stream.Dispose();

            var threadList = threads.Select(doc => new {
                thread_id = doc.threadId,
                thread_title = Escape(doc.title),
                thread_username = Escape(doc.username),
                file_name = Uri.EscapeDataString(_threadIdToFileName[doc.threadId.ToString()])
            }).ToList();

            var model = new { threads = threadList };
            return Template.Parse(templateText).Render(model);
        }

        public async Task<string> GenerateThreadPage(JsonThreadDocument data)
        {
            var converter = new ContentConverter(_threadIdToFileName);
            var responses = data.responses.Select(res => {
                return new {
                    sequence = res.sequence.ToString(),
                    username = Escape(res.username),
                    user_id = Escape(res.userId),
                    created_at = Tuna(res.createdAt),
                    content = converter.ConvertContent(res.content, data, res),
                    thread_id = res.threadId.ToString(),
                    attachment = string.IsNullOrEmpty(res.attachment) ? "" : res.attachment
                };
            }).ToList();

            var model = new {
                board_id = Escape(data.boardId),
                thread_id = data.threadId.ToString(),
                title = Escape(data.title),
                username = Escape(data.username),
                created_at = Tuna(data.createdAt),
                updated_at = Tuna(data.updatedAt),
                size = data.size.ToString(),
                responses = responses
            };

            var assembly = typeof(Exporter).Assembly;
            var stream = assembly.GetManifestResourceStream("DoTuna.Templates.thread.html");
            var reader = new StreamReader(stream, Encoding.UTF8);
            var templateText = await reader.ReadToEndAsync();
            reader.Dispose();
            stream.Dispose();

            return Template.Parse(templateText).Render(model);
        }
        void CopyRequiredImages(List<string> requireImg)
        {
            if (requireImg.Count == 0) return;
            string dataDir = Path.Combine(ResultPath, "data");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            foreach (var imgFile in requireImg.Distinct())
            {
                var src = Path.Combine(SourcePath, "data", imgFile);
                var dst = Path.Combine(dataDir, imgFile);
                if (File.Exists(src))
                {
                    File.Copy(src, dst, true);
                }
            }
        }

        string Tuna(DateTime time)
        {
            return time.AddHours(9).ToString("yyyy-MM-dd '('ddd')' HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                .Replace("Mon", "월")
                .Replace("Tue", "화")
                .Replace("Wed", "수")
                .Replace("Thu", "목")
                .Replace("Fri", "금")
                .Replace("Sat", "토")
                .Replace("Sun", "일");
        }

        static string Escape(string? s)
        {
            return System.Net.WebUtility.HtmlEncode(s ?? "");
        }
    }
}
