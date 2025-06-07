using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Scriban;

namespace DoTuna
{
    public class ScribanRenderer
    {
        private readonly Dictionary<string, string> _threadIdToFileName;
        public ScribanRenderer(Dictionary<string, string> threadIdToFileName)
        {
            _threadIdToFileName = threadIdToFileName;
        }

        public async Task<string> RenderIndexPageAsync(List<JsonIndexDocument> threads)
        {
            var model = new { threads = threads.Select(doc => new {
                thread_id = doc.threadId,
                thread_title = Exporter.Escape(doc.title),
                thread_username = Exporter.Escape(doc.username),
                file_name = Uri.EscapeDataString(_threadIdToFileName.TryGetValue(doc.threadId.ToString(), out var f) ? f : doc.threadId + ".html")
            }).ToList() };
            return await RenderTemplateFromResourceAsync("DoTuna.Templates.index.html", model);
        }

        public async Task<string> RenderThreadPageAsync(JsonThreadDocument threadModel, object responsesModel)
        {
            var model = new {
                board_id = Exporter.Escape(threadModel.boardId),
                thread_id = threadModel.threadId.ToString(),
                title = Exporter.Escape(threadModel.title),
                username = Exporter.Escape(threadModel.username),
                created_at = Exporter.Tuna(threadModel.createdAt),
                updated_at = Exporter.Tuna(threadModel.updatedAt),
                size = threadModel.size.ToString(),
                responses = responsesModel
            };
            return await RenderTemplateFromResourceAsync("DoTuna.Templates.thread.html", model);
        }

        private async Task<string> RenderTemplateFromResourceAsync(string resourceName, object model)
        {
            var assembly = typeof(ScribanRenderer).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var templateText = await reader.ReadToEndAsync();
            return Template.Parse(templateText).Render(model);
        }
    }
}
