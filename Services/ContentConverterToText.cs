namespace DoTuna
{
    public class ContentConverterToText : ContentConverter
    {
        private string _url;
        public ContentConverterToText(ThreadFileNameMap fileNameMap, string url) : base(fileNameMap)
        {
            _url = url;
        }

        public override string ConvertContent(string content, int threadId)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = FixBr(content);
            content = ConvertAnchors(content, threadId);
            content = ConvertTunagroundLinks(content);
            content = ConvertCard2Links(content);
            content = ConvertCard2QueryLinks(content);
            return content;
        }
        protected override string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal)
        {
            var anchor = string.IsNullOrEmpty(resNo) ? GetFileName(threadId) : $"{GetFileName(threadId)}#response_{resNo}";
            return $"{_url}/{anchor}";
        }
    } 
}
