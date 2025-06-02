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
        private HashSet<JsonIndexDocument> _checked = new HashSet<JsonIndexDocument>();

        public IEnumerable<JsonIndexDocument> All { get => _indexRepository.Get(); }
        public IEnumerable<JsonIndexDocument> Checked { get => _checked.OrderBy(x => x.threadId); }
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

            if (isCheck)
            {
                _checked.Add(doc); // 이미 존재하면 추가되지 않음
            }
            else
            {
                _checked.Remove(doc);
            }
        }
    }
}
