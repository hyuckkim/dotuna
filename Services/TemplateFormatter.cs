using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DoTuna
{
    public static class TemplateFormatter
    {
        public static string Format(string template, Dictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            var regex = new Regex(@"\{(\w+)(?: ([^:}]+))?(?::([^}]+))?\}");

            return regex.Replace(template, match =>
            {
                var key = match.Groups[1].Value;
                var option = match.Groups[2].Success ? match.Groups[2].Value : "";
                var fallback = match.Groups[3].Success ? match.Groups[3].Value : "";

                if (!values.TryGetValue(key, out var value) || string.IsNullOrEmpty(value))
                    value = fallback;

                if (string.IsNullOrEmpty(value))
                    return "";

                return string.IsNullOrEmpty(option)
                    ? value
                    : ApplyTruncateWithOmit(value, option);
            });
        }

        private static string ApplyTruncateWithOmit(string input, string option)
        {
            var opt = ParseOption(option);
            int len = input.Length;

            if (opt.FrontCount + opt.BackCount >= len || (opt.FrontCount == 0 && opt.BackCount == 0))
                return input;

            if (opt.FrontCount > 0 && opt.BackCount > 0)
                return input.Substring(0, opt.FrontCount) + opt.OmitString + input.Substring(len - opt.BackCount);
            else if (opt.FrontCount > 0)
                return input.Substring(0, opt.FrontCount) + opt.OmitString;
            else // back only
                return opt.OmitString + input.Substring(len - opt.BackCount);
        }

        private class SubstringOption
        {
            public int FrontCount = 0;
            public int BackCount = 0;
            public string OmitString = "";
        }

        private static SubstringOption ParseOption(string option)
        {
            var opt = new SubstringOption();
            if (string.IsNullOrEmpty(option)) return opt;

            var regex = new Regex(@"^(\d*)(.*?)(\d*)$");
            var match = regex.Match(option);

            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int front)) opt.FrontCount = front;
                if (int.TryParse(match.Groups[3].Value, out int back)) opt.BackCount = back;
                opt.OmitString = match.Groups[2].Value;
            }

            return opt;
        }
    }
}