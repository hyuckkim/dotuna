using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Scriban;
using System.Threading;
using System.Threading.Tasks;

namespace DoTuna
{
    public class Exporter
    {
        public string SourcePath { get; set; } = string.Empty;
        public string ResultPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Result");
        public string TitleTemplate { get; set; } = "{id}";

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            string indexPath = Path.Combine(ResultPath, "index.html");

            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);

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

                string jsonPath = Path.Combine(ResultPath, $"{doc.getTemplateName(TitleTemplate)}.html");
                var threadHtml = await GenerateThreadPage(content);
                await Task.Run(() => File.WriteAllText(jsonPath, threadHtml));

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
                file_name = doc.getTemplateName(this.TitleTemplate) + ".html"
            }).ToList();

            var model = new { threads = threadList };
            return Template.Parse(templateText).Render(model);
        }

        async Task<string> GenerateThreadPage(JsonThreadDocument data)
        {
            // 링크/앵커/줄바꿈 등 tunaground 스타일 서버사이드 변환
            var responses = data.responses.Select(res => new {
                sequence = res.sequence.ToString(),
                username = Escape(res.username),
                user_id = Escape(res.userId),
                created_at = Tuna(res.createdAt),
                content = ConvertContent(res.content, data, res),
                thread_id = res.threadId.ToString()
            }).ToList();
        // 서버사이드에서 tunaground 스타일 링크/앵커/줄바꿈 변환
        string ConvertContent(string content, JsonThreadDocument thread, Response res)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            // <br> 보정 (JS fixBr)
            content = System.Text.RegularExpressions.Regex.Replace(content, @"<br\s*/?>", "\n<br>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"([a-z]*)&gt;([0-9]*)&gt;([0-9]*)-?([0-9]*)",
                m => {
                    var threadId = m.Groups[2].Value == "" ? thread.threadId.ToString() : m.Groups[2].Value;
                    var responseStart = m.Groups[3].Value;
                    if (string.IsNullOrEmpty(threadId) && string.IsNullOrEmpty(responseStart))
                        return m.Value;
                    var inPageAnchor = $"response_{responseStart}";
                    // 같은 문서 내 앵커
                    return $"<a href=\"#{inPageAnchor}\">{m.Value}</a>";
                },
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"https?://bbs.tunaground.net/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => $"<a href=\"{m.Groups[2].Value}.html#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => $"<a href=\"{m.Groups[2].Value}.html#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php\\?bbs=([a-z]+)&amp;card_number=([0-9]+)([\S]*)",
                m => $"<a href=\"{m.Groups[2].Value}.html\" target=\"_blank\">{m.Value}</a>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            // 일반 링크 변환 (JS applyLink)
            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"https?://((?!www\.youtube\.com/embed/|bbs.tunaground.net|tunaground.co)(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b[-a-zA-Z0-9가-힣()@:%_\+;.~#?&//=]*)",
                m => $"<a href=\"{m.Value}\" target=\"_blank\">{m.Value}</a>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            return content;
        }

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
