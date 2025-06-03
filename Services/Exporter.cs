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
                await Task.Run(() => File.WriteAllText(jsonPath, GenerateThreadPage(content)));

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

        string GenerateThreadPage(JsonThreadDocument data)
        {
            var sb = new StringBuilder();
            sb.Append("<html lang=\"ko\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=no\" name=\"viewport\">");
            sb.Append($"<title>{Escape(data.title)}</title>");
            sb.Append("<style>@font-face{font-family:Saitamaar;src:url(https://tunaground.github.io/AA/HeadKasen.woff2)format(\"woff2\"),url(https://tunaground.github.io/AA/HeadKasen.ttf)format(\"ttf\");font-display:swap}@font-face{font-family:Saitamaar;src:url(https://cdn.jsdelivr.net/fontsource/fonts/nanum-gothic-coding@latest/korean-400-normal.woff2)format(\"woff2\"),url(https://cdn.jsdelivr.net/fontsource/fonts/nanum-gothic-coding@latest/korean-400-normal.woff)format(\"woff\");font-display:swap;unicode-range:U+AC00-D7A3,U+3130-318F}body{background-color:#f0f0f0;margin:0;font-family:sans-serif}img{max-height:10em}img:hover{max-width:100%;max-height:50em}.thread{padding-bottom:.4em}.thread_header{color:#fff;background-color:#000;padding:1em}.thread_title{font-size:2em;font-weight:700}.response_list{margin:0;padding:0}.response{background-color:#ffffffb3;border:1px solid #000;margin:.6em;list-style:none}.response_header{background-color:#0003;border-bottom:1px dashed #000;padding:.4em;font-size:.9em}.response_header p{margin:0}.response_body{overflow-wrap:break-word;padding:.4em;font-family:Saitamaar,sans-serif;font-size:.9em;line-height:1.125em}.mona{white-space:nowrap;background-color:#fff;font-family:Saitamaar,sans-serif;line-height:1.125em;overflow:auto hidden}span.spoiler{color:#0000}span.spoiler::selection{color:#fff;background-color:#000}</style></head><body><article>");
            sb.Append($"<div class=\"thread_header\"><div class=\"thread_title\">{Escape(data.boardId)}&gt;{data.threadId}&gt; {Escape(data.title)} ({data.size})</div><div class=\"thread_username\">{Escape(data.username)}</div><div class=\"thread_date\">{Tuna(data.createdAt)} - {Tuna(data.updatedAt)}</div></div>");
            sb.Append("<div class=\"thread_body\"><ul class=\"response_list\">");

            foreach (var response in data.responses)
            {
                sb.Append(MakeHtmlResponse(response));
            }

            sb.Append("</ul></div></div></article></body></html>");
            return sb.ToString();
        }

        string MakeHtmlResponse(Response res)
        {
            var sb = new StringBuilder();
            sb.Append($"<li class=\"response\" id=\"response_anchor_{res.threadId}_{res.sequence}\"><div class=\"response_header\"><p><b>{res.sequence}</b> {Escape(res.username)} ({Escape(res.userId)})</p><p>{Tuna(res.createdAt)}</p></div> <div class=\"response_body\">{res.content}</div></li>");
            return sb.ToString();
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
