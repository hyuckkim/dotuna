namespace DoTuna.Core
{
    public class ConverterToA : Converter
    {
        private readonly ThreadFile _origin;
        public ConverterToA(ThreadFileMap fileNameMap, ThreadFile origin) : base(fileNameMap)
        {
            _origin = origin;
        }

        public override string ConvertContent(string content, ulong threadId)
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
        protected override string MakeAnchorTag(ulong threadId, string resNo, string text, bool isExternal)
        {
            var targetAttr = isExternal ? " target=\"_blank\"" : "";
            var relativePath = HtmlIndexDocument.EncodeToHref(_origin.GetRelativePathTo(_fileNameMap.Get(threadId)));
            var anchor = string.IsNullOrEmpty(resNo) ? relativePath : $"{relativePath}#response_{resNo}";
            return $"<a href=\"{anchor}\"{targetAttr}>{text}</a>";
        }
    } 
}
