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

        public async Task Build(List<JsonIndexDocument> threads, IProgress<string> progress)
        {
            string indexPath = Path.Combine(ResultPath, "index.html");

            // threadId -> 파일명 매핑 테이블 생성
            var threadIdToFileName = threads.ToDictionary(
                doc => doc.threadId.ToString(),
                doc => doc.getTemplateName(TitleTemplate) + ".html"
            );

            if (!Directory.Exists(ResultPath))
                Directory.CreateDirectory(ResultPath);

            progress?.Report("(index.html 생성 중)");
            var indexHtml = await GenerateIndexPageAsync(threads, threadIdToFileName);
            await Task.Run(() => File.WriteAllText(indexPath, indexHtml));
            progress?.Report("(index.html 생성됨)");

            int completed = 0;
            progress?.Report($"({completed} of {threads.Count})");


            foreach (var doc in threads)
            {
                var threadPath = Path.Combine(SourcePath, $"{doc.threadId}.json");
                JsonThreadDocument content = await JsonThreadDocument.GetThreadAsync(threadPath);

                string jsonPath = Path.Combine(ResultPath, threadIdToFileName[doc.threadId.ToString()]);
                var threadHtml = await GenerateThreadPage(content, threadIdToFileName);
                await Task.Run(() => File.WriteAllText(jsonPath, threadHtml));

                Interlocked.Increment(ref completed);
                progress?.Report($"({completed} of {threads.Count})");
            }
        }

        async Task<string> GenerateIndexPageAsync(List<JsonIndexDocument> threads, Dictionary<string, string> threadIdToFileName)
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
                file_name = threadIdToFileName[doc.threadId.ToString()]
            }).ToList();

            var model = new { threads = threadList };
            return Template.Parse(templateText).Render(model);
        }

        async Task<string> GenerateThreadPage(JsonThreadDocument data, Dictionary<string, string> threadIdToFileName)
        {
            var responses = data.responses.Select(res => new {
                sequence = res.sequence.ToString(),
                username = Escape(res.username),
                user_id = Escape(res.userId),
                created_at = Tuna(res.createdAt),
                content = ConvertContent(res.content, data, res, threadIdToFileName),
                thread_id = res.threadId.ToString()
            }).ToList();
        string ConvertContent(string content, JsonThreadDocument thread, Response res, Dictionary<string, string> threadIdToFileName)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = FixBr(content);
            content = ConvertAnchors(content, thread, threadIdToFileName);
            content = ConvertTunagroundLinks(content, threadIdToFileName);
            content = ConvertCard2Links(content, threadIdToFileName);
            content = ConvertCard2QueryLinks(content, threadIdToFileName);
            content = ConvertGeneralLinks(content);
            return content;
        }

        // <br> 보정
        string FixBr(string content)
        {
            return Regex.Replace(content, @"<br\s*/?>", "\n<br>", RegexOptions.IgnoreCase);
        }

        // >>threadId>>n 앵커 변환
        string ConvertAnchors(string content, JsonThreadDocument thread, Dictionary<string, string> threadIdToFileName)
        {
            return Regex.Replace(
                content,
                @"([a-z]*)&gt;([0-9]*)&gt;([0-9]*)-?([0-9]*)",
                m => {
                    var threadId = m.Groups[2].Value == "" ? thread.threadId.ToString() : m.Groups[2].Value;
                    var fileName = threadIdToFileName.TryGetValue(threadId, out var f) ? f : threadId + ".html";
                    var responseStart = m.Groups[3].Value;
                    if (string.IsNullOrEmpty(threadId) && string.IsNullOrEmpty(responseStart))
                        return m.Value;
                    return $"<a href=\"{fileName}#response_{responseStart}\">{m.Value}</a>";
                },
                RegexOptions.IgnoreCase
            );
        }

        // bbs.tunaground.net/trace.php 링크 변환
        string ConvertTunagroundLinks(string content, Dictionary<string, string> threadIdToFileName)
        {
            return Regex.Replace(
                content,
                @"https?://bbs.tunaground.net/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    var fileName = threadIdToFileName.TryGetValue(threadId, out var f) ? f : threadId + ".html";
                    return $"<a href=\"{fileName}#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>";
                },
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php/ 링크 변환
        string ConvertCard2Links(string content, Dictionary<string, string> threadIdToFileName)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    var fileName = threadIdToFileName.TryGetValue(threadId, out var f) ? f : threadId + ".html";
                    return $"<a href=\"{fileName}#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>";
                },
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php?bbs=...&card_number=... 링크 변환
        string ConvertCard2QueryLinks(string content, Dictionary<string, string> threadIdToFileName)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php\\?bbs=([a-z]+)&amp;card_number=([0-9]+)([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    var fileName = threadIdToFileName.TryGetValue(threadId, out var f) ? f : threadId + ".html";
                    return $"<a href=\"{fileName}\" target=\"_blank\">{m.Value}</a>";
                },
                RegexOptions.IgnoreCase
            );
        }

        // 일반 링크 변환 (JS applyLink)
        string ConvertGeneralLinks(string content)
        {
            return Regex.Replace(
                content,
                @"https?://((?!www\.youtube\.com/embed/|bbs.tunaground.net|tunaground.co)(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b[-a-zA-Z0-9가-힣()@:%_\+;.~#?&//=]*)",
                m => $"<a href=\"{m.Value}\" target=\"_blank\">{m.Value}</a>",
                RegexOptions.IgnoreCase
            );
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
