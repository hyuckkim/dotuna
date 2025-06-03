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
        public IEnumerable<JsonIndexDocument> Filtered()
        {
            return All.Where(x => x.title.Contains(TitleFilter) &&
                                  x.username.Contains(AuthorFilter));
        }

        public string TitleFilter { get; set; } = string.Empty;
        public string AuthorFilter { get; set; } = string.Empty;

        public ThreadManager(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
        }
        public void Check(JsonIndexDocument doc, bool isCheck)
        {
            if (doc == null) return;

            if (isCheck)
            {
                _checked.Add(doc);
            }
            else
            {
                _checked.Remove(doc);
            }
        }
    }
}
