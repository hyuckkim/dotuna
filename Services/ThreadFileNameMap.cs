using System.Collections.Generic;
using System.Linq;

namespace DoTuna
{
    public class ThreadFileNameMap
    {
        private readonly Dictionary<string, string> _map;
        public int Size { get => _map.Values.Distinct().Count(); }

        public ThreadFileNameMap(IEnumerable<JsonIndexDocument> threads, string titleTemplate)
        {
            _map = threads.ToDictionary(
                doc => doc.threadId.ToString(),
                doc => doc.getTemplateName(titleTemplate) + ".html"
            );
        }

        public string GetFileName(string threadId)
        {
            return _map.TryGetValue(threadId, out var f) ? f : threadId + ".html";
        }

        public string this[string threadId] => GetFileName(threadId);

        public IReadOnlyDictionary<string, string> AsDictionary() => _map;
    }
}
