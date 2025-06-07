using System;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace DoTuna
{
    public class ContentConverter
    {
        private Dictionary<string, string> _threadIdToFileName;
        public ContentConverter(Dictionary<string, string> threadIdToFileName)
        {
            _getFileName = getFileName;
            _getThreadId = getThreadId;
        }

        public string ConvertContent(string content, JsonThreadDocument thread, Response res)
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

        // <br> 보정
        string FixBr(string content)
        {
            return Regex.Replace(content, @"<br\s*/?>", "\n<br>", RegexOptions.IgnoreCase);
        }

        // >>threadId>>n 앵커 변환
        string ConvertAnchors(string content, JsonThreadDocument thread)
        {
            return Regex.Replace(
                content,
                @"([a-z]*)&gt;([0-9]*)&gt;([0-9]*)-?([0-9]*)",
                m => {
                    var threadId = m.Groups[2].Value == "" ? thread.threadId.ToString() : m.Groups[2].Value;
                    var responseStart = m.Groups[3].Value;
                    if (string.IsNullOrEmpty(threadId) && string.IsNullOrEmpty(responseStart))
                        return m.Value;
                    return $"<a href=\"{GetFileName(threadId)}#response_{responseStart}\">{m.Value}</a>";
                },
                RegexOptions.IgnoreCase
            );
        }

        // bbs.tunaground.net/trace.php 링크 변환
        string ConvertTunagroundLinks(string content)
        {
            return Regex.Replace(
                content,
                @"https?://bbs.tunaground.net/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => $"<a href=\"{GetFileName(m.Groups[2].Value)}#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>",
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php/ 링크 변환
        string ConvertCard2Links(string content)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => $"<a href=\"{GetFileName(m.Groups[2].Value)}#response_{m.Groups[3].Value}\" target=\"_blank\">{m.Value}</a>",
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php?bbs=...&card_number=... 링크 변환
        string ConvertCard2QueryLinks(string content)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground.co/card2?post/trace.php\\?bbs=([a-z]+)&amp;card_number=([0-9]+)([\S]*)",
                m => $"<a href=\"{GetFileName(m.Groups[2].Value)}\" target=\"_blank\">{m.Value}</a>",
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

        string GetFileName(string threadId) => Uri
            .EscapeDataString(_threadIdToFileName.TryGetValue(threadId, out var f) ? f : threadId + ".html");
    }
}
