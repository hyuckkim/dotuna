using System;

namespace DoTuna.Core
{
    public class AppSetting
    {
        private static readonly AppSetting _instance = new AppSetting();

        public static AppSetting Instance => _instance;

        public string Pattern { get; set; } = "{id}";
        public bool SingleHTML { get; set; } = false;
        public bool UseCssFile { get; set; } = false;
        public bool PreloadIndex { get; set; } = false;
        public int ThreadCount { get; set; } = Environment.ProcessorCount * 2;

        private AppSetting()
        {

        }
    }
}