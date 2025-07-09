namespace DoTuna
{
    public class Setting
    {
        private static readonly Setting _instance = new Setting();

        public static Setting Instance => _instance;

        public string Pattern { get; set; }

        private Setting()
        {
            Pattern = "{id}";
        }
    }
}