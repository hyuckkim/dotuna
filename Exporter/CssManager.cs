using System.IO;
using System.Text;

namespace DoTuna
{
    public class CssManager
    {
        public static CssManager Instance { get; } = new CssManager();

        private string archive;
        private CssManager()
        {
            archive = GetResource("archive");
        }
        private string GetResource(string templateName)
        {
            var assembly = typeof(CssManager).Assembly;
            using var stream = assembly.GetManifestResourceStream($"DoTuna.Templates.{templateName}.css");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public string GetLink()
        {
            if (Setting.Instance.UseCssFile)
            {
                return "<link rel=\"stylesheet\" href=\"style.css\">";
            }
            else
            {
                return "<style>" + archive + "</style>";
            }
        }
        public string GetContent()
        {
            return archive;
        }
    }
}