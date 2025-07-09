namespace DoTuna
{
    public class Setting
    {
        private static readonly Setting _instance = new Setting();

        public static Setting Instance => _instance;

        public string Pattern { get; set; } = "{id}";
        public bool SingleHTML { get; set; } = false;

        private Setting()
        {

        }
    }
}