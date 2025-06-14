using System.Text.RegularExpressions;

namespace DoTuna
{
    public class ContentConverterToText : ContentConverter
    {
        public string Url { get; set; } = string.Empty;
        public ContentConverterToText(ThreadFileNameMap fileNameMap, string url) : base(fileNameMap)
        {
            Url = url;
        }

        public override string ConvertContent(string content, int threadId)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            content = FixBr(content);
            content = ConvertRawAnchors(content, threadId);
            content = ConvertAnchors(content, threadId);
            content = ConvertTunagroundLinks(content);
            content = ConvertCard2Links(content);
            content = ConvertCard2QueryLinks(content);
            content = ConvertArchives(content);
            return content;
        }
        
        // >>threadId>>n 앵커 변환 (&gt;가 아니라 > 확인)
        protected string ConvertRawAnchors(string content, int threadId)
        {
            return Regex.Replace(
                content,
                @"([a-z]*)>([0-9]*)>([0-9]*)-?([0-9]*)",
                m => {
                    var id = m.Groups[2].Value == "" ? threadId.ToString() : m.Groups[2].Value;
                    var responseStart = m.Groups[3].Value;
                    if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(responseStart))
                        return m.Value;
                    return MakeAnchorTag(id, responseStart, m.Value, false);
                },
                RegexOptions.IgnoreCase
            );
        }

        protected string ConvertArchives(string content)
        {
            return Regex.Replace(
                content,
                @"https?://archive\.tunaground\.net/([a-z]+)/([0-9]+)\.html",
                m => {
                    var threadId = m.Groups[2].Value;
                    // resNo가 없는 경우 빈 문자열 전달
                    return MakeAnchorTag(threadId, "", m.Value, true);
                },
                RegexOptions.IgnoreCase
            );
        }
        protected override string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal)
        {
            var anchor = string.IsNullOrEmpty(resNo) ? GetFileName(threadId) : $"{GetFileName(threadId)}#response_{resNo}";
            return $"{Url}/{anchor}";
        }
    } 
}
