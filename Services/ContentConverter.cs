using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DoTuna
{
    public abstract class ContentConverter
    {
        protected readonly ThreadFileNameMap _fileNameMap;
        public ContentConverter(ThreadFileNameMap fileNameMap)
        {
            _fileNameMap = fileNameMap;
        }


        // <br> 보정
        protected string FixBr(string content)
        {
            return Regex.Replace(content, @"<br\s*/?>", "\n<br>", RegexOptions.IgnoreCase);
        }

        // >>threadId>>n 앵커 변환
        protected string ConvertAnchors(string content, int threadId)
        {
            return Regex.Replace(
                content,
                @"([a-z]*)&gt;([0-9]*)&gt;([0-9]*)-?([0-9]*)",
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

        // bbs.tunaground.net/trace.php 링크 변환
        protected string ConvertTunagroundLinks(string content)
        {
            return Regex.Replace(
                content,
                @"https?://bbs.tunaground.net/trace.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    var resNo = m.Groups[3].Value;
                    return MakeAnchorTag(threadId, resNo, m.Value, true);
                },
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php/ 링크 변환
        protected string ConvertCard2Links(string content)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground\.co/card2\?post/trace\.php/([a-z]+)/([0-9]+)/([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    var resNo = m.Groups[3].Value;
                    return MakeAnchorTag(threadId, resNo, m.Value, true);
                },
                RegexOptions.IgnoreCase
            );
        }

        // tunaground.co/card2?post/trace.php?bbs=...&card_number=... 링크 변환
        protected string ConvertCard2QueryLinks(string content)
        {
            return Regex.Replace(
                content,
                @"https?://tunaground\.co/card2\?post/trace\.php\?bbs=([a-z]+)&amp;card_number=([0-9]+)([\S]*)",
                m => {
                    var threadId = m.Groups[2].Value;
                    // resNo가 없는 경우 빈 문자열 전달
                    return MakeAnchorTag(threadId, "", m.Value, true);
                },
                RegexOptions.IgnoreCase
            );
        }

        // 일반 링크 변환 (JS applyLink)
        protected string ConvertGeneralLinks(string content)
        {
            return Regex.Replace(
            content,
            @"https?://((?!www\.youtube\.com/embed/|bbs.tunaground.net|tunaground.co)(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b[-a-zA-Z0-9가-힣()@:%_\+;.~#?&//=]*)",
            m => $"<a href=\"{m.Value}\" target=\"_blank\">{m.Value}</a>",
            RegexOptions.IgnoreCase
            );
        }

        protected string GetFileName(string threadId) => Uri
            .EscapeDataString(_fileNameMap.GetFileName(threadId));

        public abstract string ConvertContent(string content, int threadId);
        protected abstract string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal);
    }
}
