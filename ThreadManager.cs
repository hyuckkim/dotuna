using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoTuna
{
    public class ThreadManager
    {
        private IIndexRepository _indexRepository;
        public IEnumerable<JsonIndexDocument> All { get => _indexRepository.Get(); }
        public IEnumerable<JsonIndexDocument> Checked { get => All.Where(x => x.IsCheck).OrderBy(x => x.threadId); }
        public IEnumerable<JsonIndexDocument> Filtered(string title, string author)
        {
            return All.Where(x => x.title.Contains(title) &&
                                  x.username.Contains(author));
        }

        public ThreadManager(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
        }
        public void Check(JsonIndexDocument doc, bool isCheck)
        {
            if (doc == null) return;
            doc.IsCheck = isCheck;
        }
    }
}
