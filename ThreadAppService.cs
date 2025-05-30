using System;
using System.Collections.Generic;
using System.Linq;
using DoTuna.Thread;

public class ThreadAppService
{
    public void OpenFolder(string path)
    {
        ThreadManager.Open(path);
    }

    public IEnumerable<JsonIndexDocument> GetFiltered(string title, string author)
    {
        return Filter(title, author);
    }

    public void CheckFiltered(string title, string author, bool select)
    {
        foreach (var doc in Filter(title, author))
        {
            doc.IsCheck = select;
        }
    }

    public void CheckAll(bool select)
    {
        foreach (var doc in ThreadManager.Index)
        {
            doc.IsCheck = select;
        }
    }

    private IEnumerable<JsonIndexDocument> Filter(string title, string author)
    {
        return ThreadManager.Index
            .Where(d => d.title.Contains(title))
            .Where(d => d.username.Contains(author));
    }
}
