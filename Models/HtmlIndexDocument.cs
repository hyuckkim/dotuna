namespace DoTuna
{
    public class HtmlIndexDocument
    {
#pragma warning disable IDE1006 // JS 속성
        public string thread_id { get; set; } = "";
        public string thread_title { get; set; } = "";
        public string thread_username { get; set; } = "";
        public string file_name { get; set; } = "";
#pragma warning restore IDE1006 // JS 속성

        public static HtmlIndexDocument FromJson(JsonIndexDocument doc, ThreadFileNameMap fileNameMap)
        {
            return new HtmlIndexDocument
            {
                thread_id = doc.threadId.ToString(),
                thread_title = ScribanRenderer.Escape(doc.title),
                thread_username = ScribanRenderer.Escape(doc.username),
                file_name = FileHelper.EncodeToHref(fileNameMap.GetFileName(doc.threadId))
            };
        }
    }
}