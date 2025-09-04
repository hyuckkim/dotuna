using System.Collections.Generic;
using System.Linq;

namespace DoTuna.Core
{

    public class ThreadFileMap
    {
        private readonly Dictionary<ulong, ThreadFile> _map;
        public bool IsUnique { get => _map.Values.Select(x => x.FileName).Distinct().Count() == _map.Count; }

        public ThreadFileMap(IEnumerable<JsonIndexDocument> threads, string pattern)
        {
            _map = threads.ToDictionary(
                doc => doc.threadId,
                doc => new ThreadFile(doc, pattern)
            );
        }

        public string GetFileName(ulong threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f.FileName : threadId + ".html";
        }

        public ThreadFile Get(ulong threadId)
        {
            if (_map.TryGetValue(threadId, out var f))
                return f;
            else throw new KeyNotFoundException("Thread ID not found: " + threadId);
        }

        public string this[ulong threadId] => GetFileName(threadId);
    }
}
