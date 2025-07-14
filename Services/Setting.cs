using System;

namespace DoTuna
{
    public class Setting
    {
        private static readonly Setting _instance = new Setting();

        public static Setting Instance => _instance;

        public string Pattern { get; set; } = "{id}";
        public bool SingleHTML { get; set; } = false;
        public bool UseCssFile { get; set; } = false;
        public bool PreloadIndex { get; set; } = false;
        public int ThreadCount { get; set; } = Environment.ProcessorCount * 2;

        private Setting()
        {

        }
    }
}