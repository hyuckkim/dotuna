namespace DoTuna
{
    public class ContentConverterToA : ContentConverter
    {
        public ContentConverterToA(ThreadFileNameMap fileNameMap) : base(fileNameMap)
        {
        }

        public override string ConvertContent(string content, JsonThreadDocument thread, Response res)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = FixBr(content);
            content = ConvertAnchors(content, thread);
            content = ConvertTunagroundLinks(content);
            content = ConvertCard2Links(content);
            content = ConvertCard2QueryLinks(content);
            content = ConvertGeneralLinks(content);
            return content;
        }
        protected override string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal)
        {
            var targetAttr = isExternal ? " target=\"_blank\"" : "";
            return $"<a href=\"{GetFileName(threadId)}#response_{resNo}\"{targetAttr}>{text}</a>";
        }
    } 
}
