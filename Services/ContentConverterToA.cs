namespace DoTuna
{
    public class ContentConverterToA : ContentConverter
    {
        public ContentConverterToA(ThreadFileNameMap fileNameMap) : base(fileNameMap)
        {
        }

        protected override string MakeAnchorTag(string threadId, string resNo, string text, bool isExternal)
        {
            return $"<a href=\"{GetFileName(threadId)}#response_{resNo}\">{text}</a>";
        }
    } 
}
