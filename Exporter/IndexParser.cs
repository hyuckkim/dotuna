using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoTuna
{
    public class IndexParser
    {
        public static async Task<List<HtmlIndexDocument>> ParseIndex(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return new List<HtmlIndexDocument>();
            }
            var html = await FileHelper.ReadAllTextAsync(path);
            var match = Regex.Match(html, @"const data = \[(.*?)\];", RegexOptions.Singleline);
            if (!match.Success)
            {
                return new List<HtmlIndexDocument>();
            }

            string dataContent = match.Groups[1].Value;

            // 1. 속성명에 큰따옴표 씌우기 (key: -> "key":)
            string jsonContent = Regex.Replace(dataContent, @"(\w+):", @"""$1"":");

            // 2. 문자열 값 안전하게 escape 처리
            // 문자열 값은 "..."로 되어 있다고 가정
            jsonContent = Regex.Replace(jsonContent, @"""([^""]*?)""", m => {
                string s = m.Value; // ex: "some "text" here"
                // 내부 큰따옴표는 \"로, 역슬래시는 \\로, 줄바꿈은 \n으로 변환
                string inner = s.Substring(1, s.Length - 2);
                inner = inner.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");
                return $"\"{inner}\"";
            });

            // 3. 마지막 쉼표 제거
            jsonContent = "[" + jsonContent.Trim().TrimEnd(',') + "]";

            // 4. JSON 파싱
            return JsonConvert.DeserializeObject<List<HtmlIndexDocument>>(jsonContent) ?? new List<HtmlIndexDocument>();
        }
    }
}
