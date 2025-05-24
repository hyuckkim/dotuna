using System.IO;
using System.Text;
using DoTuna.Thread;

namespace Dotuna.Export 
{
    public static class ExportManager
    {
        public static readonly string ReusltPath = Path.Combine(Directory.GetCurrentDirectory(), "Result");
        
        public static async Task Build(IProgress<string>? progress = null)
        {
            string indexPath = Path.Combine(ReusltPath, "index.html");

            if (!Directory.Exists(ReusltPath))
            {
                Directory.CreateDirectory(ReusltPath);
            }
            progress?.Report("(index.html 생성 중)");
            await File.WriteAllTextAsync(indexPath, GenerateIndexPage());
            progress?.Report("(index.html 생성됨)");
            
            var tasks = new List<Task>();
            var completed = 0;
            var total = ThreadManager.Index.Where(x => x.IsCheck).Count();
            progress?.Report($"({completed} of {total})");

            foreach (var thread in ThreadManager.Index.Where(x => x.IsCheck).OrderBy(x => x.threadId))
            {
                string threadPath = Path.Combine(ReusltPath, $"{thread.threadId}.html");
                await File.WriteAllTextAsync(threadPath, await GenerateThreadPage(thread.threadId));
                Interlocked.Increment(ref completed);
                progress?.Report($"({completed} of {total})");
            }
        }
        static string GenerateIndexPage() 
        {
            var sb = new StringBuilder();
            sb.Append("<html lang=\"ko\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=no\" name=\"viewport\"><title>anchor</title><style>");
            sb.Append("body{background-color:#f0f0f0;margin:0;font-family:sans-serif}nav{background-color:#000;padding:.5em 1em}.search_container{display:flex}.search{flex-grow:1;margin:.5em;font-size:1em}.pagination{margin:.5em;line-height:1.5em}.btn_pg{margin:.2em}ul{margin:.7em .5em;padding:0;display:flex}li{list-style:none}.thread_id{min-width:6em}.thread_username{margin-left:auto;margin-right:0}@media (width<=768px){.thread_id{display:none}");
            sb.Append("</style></head>");
            sb.Append("<body><nav><input class=\"search_container\" type=\"text\" placeholder=\"검색\" oninput=\"Build()\"></nav>");
            sb.Append("<div class=\"pagination\" id=\"pagination\"></div>");
            sb.Append("<div id=\"thread_list\"></div>");
            sb.Append("<script>");
            sb.Append(MakeJsIndex());
            sb.Append("window.Build = (pI = 0) => { let pagination = ''; for (let i = 0; i < filtered().length / 100; i++) { pagination += `<button class=\"btn_pg\" onclick=\"Build(${i})\">${i + 1}</button>`; } document.querySelector('#pagination').innerHTML = pagination; let threadList = ''; filtered().slice(pI * 100, (pI + 1) * 100).forEach((item) => { threadList += `<div><ul><li class=\"thread_id\">${item.thread_id}</li><li class=\"thread_title\"> <a href=\"${item.thread_id}.html\" target=\"_blank\">${item.thread_title}</a></li><li class=\"thread_username\">${item.thread_username}</li></ul></div>` }); document.querySelector('#thread_list').innerHTML = threadList; }; const sv = () => document.querySelector(\".search_container\").value; const filtered = () => data.filter((item) => item.thread_title.includes(sv()) || item.thread_username.includes(sv())); Build(); </script>");
            sb.Append("</body></html>");
            return sb.ToString();
        }
        static string MakeJsIndex()
        {
            var sb = new StringBuilder();
            sb.Append("const data = [");
            foreach (var index in ThreadManager.Index.Where(x => x.IsCheck).OrderBy(x => x.threadId))
            {
                sb.Append("{ ");
                sb.Append($"thread_id: \"{index.threadId}\", thread_title: \"{index.title}\", thread_username: \"{index.username}\"");
                sb.Append(" },");
            }
            sb.Append("];");
            return sb.ToString();
        }
        static async Task<string> GenerateThreadPage(int id)
        {
            var data = await ThreadManager.GetThreadAsync(id);
            var sb = new StringBuilder();
            sb.Append("<html lang=\"ko\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=no\" name=\"viewport\">");
            sb.Append($"<title>{data.title}</title>");
            sb.Append("<style>@font-face{font-family:Saitamaar;src:url(https://tunaground.github.io/AA/HeadKasen.woff2)format(\"woff2\"),url(https://tunaground.github.io/AA/HeadKasen.ttf)format(\"ttf\");font-display:swap}@font-face{font-family:Saitamaar;src:url(https://cdn.jsdelivr.net/fontsource/fonts/nanum-gothic-coding@latest/korean-400-normal.woff2)format(\"woff2\"),url(https://cdn.jsdelivr.net/fontsource/fonts/nanum-gothic-coding@latest/korean-400-normal.woff)format(\"woff\");font-display:swap;unicode-range:U+AC00-D7A3,U+3130-318F}body{background-color:#f0f0f0;margin:0;font-family:sans-serif}img{max-height:10em}img:hover{max-width:100%;max-height:50em}.thread{padding-bottom:.4em}.thread_header{color:#fff;background-color:#000;padding:1em}.thread_title{font-size:2em;font-weight:700}.response_list{margin:0;padding:0}.response{background-color:#ffffffb3;border:1px solid #000;margin:.6em;list-style:none}.response_header{background-color:#0003;border-bottom:1px dashed #000;padding:.4em;font-size:.9em}.response_header p{margin:0}.response_body{overflow-wrap:break-word;padding:.4em;font-family:Saitamaar,sans-serif;font-size:.9em;line-height:1.125em}.mona{white-space:nowrap;background-color:#fff;font-family:Saitamaar,sans-serif;line-height:1.125em;overflow:auto hidden}span.spoiler{color:#0000}span.spoiler::selection{color:#fff;background-color:#000}</style></head><body><article>");
            sb.Append($"<div class=\"thread_header\"><div class=\"thread_title\">{data.boardId}&gt;{data.threadId}&gt; {data.title} ({data.size})</div><div class=\"thread_username\">{data.username}</div><div class=\"thread_date\">{data.createdAt.Tuna()} - {data.updatedAt.Tuna()}</div></div>");
            sb.Append("<div class=\"thread_body\"><ul class=\"response_list\">");

            foreach (var response in data.responses)
            {
                sb.Append(MakeHtmlResponse(response));
            }
            sb.Append("</ul></div></div></article></body></html>");
            return sb.ToString();
        }
        static string MakeHtmlResponse(Response res)
        {
            var sb = new StringBuilder();
            sb.Append($"<li class=\"response\" id=\"response_anchor_{res.threadId}_{res.sequence}\"><div class=\"response_header\"><p><b>{res.sequence}</b> {res.username} ({res.userId})</p><p>{res.createdAt.Tuna()}</p></div> <div class=\"response_body\">{res.content}</div></li>");
            return sb.ToString();
        }
        static string Tuna(this DateTime time)
        {
            return time.AddHours(9).ToString("yyyy-MM-dd '('ddd')' HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                .Replace("Mon", "월")
                .Replace("Tue", "화")
                .Replace("Wed", "수")
                .Replace("Thu", "목")
                .Replace("Fri", "금")
                .Replace("Sat", "토")
                .Replace("Sun", "일");
        }
    }
}
