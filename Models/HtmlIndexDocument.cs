using System;
using System.Net;
using System.Text;

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
                thread_title = WebUtility.HtmlEncode(doc.title),
                thread_username = WebUtility.HtmlEncode(doc.username),
                file_name = EncodeToHref(fileNameMap.GetFileName(doc.threadId))
            };
        }
        
        public static string EncodeToHref(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (IsTargetChar(c))
                    sb.Append($"%{(int)c:X2}");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsTargetChar(char c)
        {
            // 공백
            if (c == ' ') return true;

            // 제어 문자
            if (c == '\n' || c == '\r' || c == '\t') return true;

            // 특수 문자
            char[] specials = { '!', '@', '#', '%', '^', '&', '*', '(', ')' };
            return Array.IndexOf(specials, c) >= 0;
        }
    }
}