namespace DoTuna
{
    public class ContentConverterToA : ContentConverter
    {
        public ContentConverterToA(ThreadFileNameMap fileNameMap) : base(fileNameMap)
        {
        }

        public override string ConvertContent(string content, int threadId)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = FixBr(content);
            content = ConvertAnchors(content, threadId);
            content = ConvertTunagroundLinks(content);
            content = ConvertCard2Links(content);
            content = ConvertCard2QueryLinks(content);
            content = ConvertGeneralLinks(content);
            return content;
        }
        protected override string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal)
        {
            var targetAttr = isExternal ? " target=\"_blank\"" : "";
            var anchor = string.IsNullOrEmpty(resNo) ? GetFileName(threadId) : $"{GetFileName(threadId)}#response_{resNo}";
            return $"<a href=\"{anchor}\"{targetAttr}>{text}</a>";
        }
    } 
}
