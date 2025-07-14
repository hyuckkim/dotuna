
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoTuna
{
    public class IndexParser
    {
        public static async Task<List<HtmlIndexDocument>> ParseIndex(string path)
        {
            var html = await FileHelper.ReadAllTextAsync(path);
            var match = Regex.Match(html, @"const data = \[(.*?)\];", RegexOptions.Singleline);
            if (!match.Success)
            {
                return new List<HtmlIndexDocument>();
            }

            string dataContent = match.Groups[1].Value;

            // 2. JS 객체 표기법 → JSON 변환
            // 속성명에 따옴표 추가 (key: -> "key":)
            string jsonContent = Regex.Replace(dataContent, @"(\w+):", @"""$1"":");

            // 배열의 마지막에 쉼표 있을 경우 제거
            jsonContent = "[" + jsonContent.Trim().TrimEnd(',') + "]";

            // 3. JSON 파싱
            return JsonConvert.DeserializeObject<List<HtmlIndexDocument>>(jsonContent) ?? new List<HtmlIndexDocument>();

        }
    }
}