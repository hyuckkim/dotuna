using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;

namespace DoTuna
{
    public class ScribanRenderer
    {
        private readonly ThreadFileNameMap _fileNameMap;
        public ScribanRenderer(ThreadFileNameMap fileNameMap)
        {
            _fileNameMap = fileNameMap;
        }

        public async Task<string> RenderIndexPageAsync(List<JsonIndexDocument> threads)
        {
            var model = new { threads = threads.Select(doc => new {
                thread_id = doc.threadId,
                thread_title = Escape(doc.title),
                thread_username = Escape(doc.username),
                file_name = Uri.EscapeDataString(_fileNameMap.GetFileName(doc.threadId.ToString()))
            }).ToList() };
            return await RenderTemplateFromResourceAsync("DoTuna.Templates.index.html", model);
        }

        public async Task<string> RenderThreadPageAsync(JsonThreadDocument threadModel)
        {
            var responses = BuildResponses(threadModel);
            var model = new {
                board_id = Escape(threadModel.boardId),
                thread_id = threadModel.threadId.ToString(),
                title = Escape(threadModel.title),
                username = Escape(threadModel.username),
                created_at = Tuna(threadModel.createdAt),
                updated_at = Tuna(threadModel.updatedAt),
                size = threadModel.size.ToString(),
                responses = responses
            };
            return await RenderTemplateFromResourceAsync("DoTuna.Templates.thread.html", model);
        }

        private List<object> BuildResponses(JsonThreadDocument data)
        {
            var converter = new ContentConverter(_fileNameMap);
            return data.responses.Select(res => new {
                sequence = res.sequence.ToString(),
                username = Escape(res.username),
                user_id = Escape(res.userId),
                created_at = Tuna(res.createdAt),
                content = converter.ConvertContent(res.content, data, res),
                thread_id = res.threadId.ToString(),
                attachment = string.IsNullOrEmpty(res.attachment) ? "" : res.attachment
            }).ToList<object>();
        }

        private async Task<string> RenderTemplateFromResourceAsync(string resourceName, object model)
        {
            var assembly = typeof(ScribanRenderer).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var templateText = await reader.ReadToEndAsync();
            return Template.Parse(templateText).Render(model);
        }
        static string Tuna(DateTime time)
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
