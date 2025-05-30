using DoTuna.Thread;

public class ThreadAppService
{
    public void OpenFolder(string path)
    {
        ThreadManager.Open(path);
    }

    public IEnumerable<JsonIndexDocument> GetFiltered(string title, string author)
    {
        return ThreadManager.Index
            .Where(d => d.title.Contains(title))
            .Where(d => d.username.Contains(author));
    }

    public void SelectFiltered(string title, string author, bool select)
    {
        var filtered = ThreadManager.Index
            .Where(d => d.title.Contains(title))
            .Where(d => d.username.Contains(author));

        foreach(JsonIndexDocument doc in filtered)
        {
            doc.IsCheck = select;
        }
    }
    
    public void SelectAll(bool select)
    {
        foreach(JsonIndexDocument doc in ThreadManager.Index)
        {
            doc.IsCheck = select;
        }
    }
}
